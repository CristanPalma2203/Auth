using Application.Commands.AppUser;
using Application.Dtos;
using Application.Dtos.AppUser;
using Application.Exceptions;
using Application.Mappers;
using Domain.Specifications;
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
            EnsureRolesDelTenant(message.AppUser.Roles?.Select(c => c.Id).ToList());

            foreach (var item in dbUser.Roles)
            {
                userRoleRepository.Delete(item.Id);
            }

            dbUser.AdminChangesPassword(
                message.AppUser.Name,
                message.AppUser.DepartmentId,
                message.AppUser.Password,
                message.AppUser.Roles.Select(c => c.Id).ToList(),
                message.AppUser.IsActive);

            dbUser.Dui = message.AppUser.Dui;
            dbUser.Nit = message.AppUser.Nit;
            dbUser.Phone = message.AppUser.Phone;

            appUserRepository.Update(dbUser.Id, dbUser);
            return UserMappingHelper.ToDtoResponse(dbUser, roleRepository);
        }

        private void EnsureRolesDelTenant(System.Collections.Generic.IList<int> roleIds)
        {
            if (tenantContext.IsPlatformAdmin || roleIds == null) return;
            foreach (var roleId in roleIds)
            {
                var Roles = roleRepository.GetById(roleId);
                if (Roles == null || Roles.TenantId != tenantContext.TenantId)
                    throw new HttpException(403, "Solo puede asignar roles de su empresa");
            }
        }
    }
}
