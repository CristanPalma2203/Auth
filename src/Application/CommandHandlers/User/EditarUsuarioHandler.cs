using Application.Commands.AppUser;
using Application.Dtos;
using Application.Dtos.AppUser;
using Application.Exceptions;
using Application.Mappers;
using Domain.Repositories;
using Domain.Service;
using MapsterMapper;
using System.Linq;

namespace Application.CommandHandlers.AppUser
{
    public class EditUserHandler : AbstractHandler<EditUser>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IMapper mapper;
        private readonly IRoleRepository roleRepository;
        private readonly IUsuarioRolRepository userRoleRepository;
        private readonly ITenantContext tenantContext;

        public EditUserHandler(
            IAppUserRepository appUserRepository,
            IMapper mapper,
            IRoleRepository roleRepository,
            IUsuarioRolRepository userRoleRepository,
            ITenantContext tenantContext)
        {
            this.appUserRepository = appUserRepository;
            this.mapper = mapper;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(EditUser message)
        {
            var dbUser = appUserRepository.GetByIdConRoles(message.AppUser.Id);
            if (dbUser == null)
                throw new HttpException(404, "AppUser no encontrado");

            tenantContext.EnsureSameTenantOrPlatform(dbUser.TenantId);

            var access = (message.AppUser.AccessIdentifier ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(access))
                throw new HttpException(422, "El identificador de acceso es obligatorio");

            var conflict = appUserRepository.GetAll()
                .FirstOrDefault(u => u.Id != dbUser.Id
                    && u.AccessIdentifier != null
                    && u.AccessIdentifier.ToLower() == access.ToLower());
            if (conflict != null)
                throw new HttpException(422, "Ya existe un usuario con ese identificador de acceso");

            // Plataforma puede mover empresa / volver a plataforma.
            // Tenant user: empresa fija a la del actor.
            int? empresaDestino;
            if (tenantContext.IsPlatformAdmin)
            {
                if (message.AppUser.IsPlatformUser)
                    empresaDestino = null;
                else if (message.AppUser.TenantId.HasValue && message.AppUser.TenantId.Value > 0)
                    empresaDestino = message.AppUser.TenantId;
                else
                    empresaDestino = dbUser.TenantId;
            }
            else
            {
                empresaDestino = tenantContext.TenantId;
            }

            EnsureRolesDelTenant(message.AppUser.Roles?.Select(c => c.Id).ToList(), empresaDestino);

            foreach (var item in dbUser.Roles.ToList())
            {
                userRoleRepository.Delete(item.Id);
            }

            dbUser.AdminChangesPassword(
                message.AppUser.Name,
                message.AppUser.DepartmentId,
                message.AppUser.Password,
                message.AppUser.Roles.Select(c => c.Id).ToList(),
                message.AppUser.IsActive);

            dbUser.AccessIdentifier = access;
            dbUser.TenantId = empresaDestino;
            dbUser.Dui = string.IsNullOrWhiteSpace(message.AppUser.Dui) ? null : message.AppUser.Dui.Trim();
            dbUser.Nit = string.IsNullOrWhiteSpace(message.AppUser.Nit) ? null : message.AppUser.Nit.Trim();
            dbUser.Phone = string.IsNullOrWhiteSpace(message.AppUser.Phone) ? null : message.AppUser.Phone.Trim();

            appUserRepository.Update(dbUser.Id, dbUser);
            return UserMappingHelper.ToDtoResponse(dbUser, roleRepository);
        }

        private void EnsureRolesDelTenant(System.Collections.Generic.IList<int> roleIds, int? empresaDestino)
        {
            if (roleIds == null) return;

            // Usuario plataforma: roles globales (TenantId null).
            if (!empresaDestino.HasValue)
            {
                foreach (var roleId in roleIds)
                {
                    var role = roleRepository.GetById(roleId);
                    if (role == null || role.TenantId != null)
                        throw new HttpException(403, "Usuario de plataforma solo usa roles globales");
                }
                return;
            }

            foreach (var roleId in roleIds)
            {
                var role = roleRepository.GetById(roleId);
                if (role == null || role.TenantId != empresaDestino)
                    throw new HttpException(403, "Solo puede asignar roles de la empresa del usuario");
            }
        }
    }
}
