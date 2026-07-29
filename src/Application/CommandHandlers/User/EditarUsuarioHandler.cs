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
        private readonly IUsuarioRegionalRepository usuarioRegionalRepository;
        private readonly IUsuarioAreaRepository usuarioAreaRepository;
        private readonly ITenantContext tenantContext;

        public EditUserHandler(
            IUsuarioAreaRepository usuarioAreaRepository,
            IAppUserRepository appUserRepository,
            IMapper mapper,
            IRoleRepository roleRepository,
            IUsuarioRolRepository userRoleRepository,
            IUsuarioRegionalRepository usuarioRegionalRepository,
            ITenantContext tenantContext)
        {
            this.usuarioRegionalRepository = usuarioRegionalRepository;
            this.appUserRepository = appUserRepository;
            this.mapper = mapper;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            this.usuarioAreaRepository = usuarioAreaRepository;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(EditUser message)
        {
            var appUser = mapper.Map<Domain.Models.AppUser>(message.AppUser);
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
            LimpiarUsuarioRegional(message.AppUser.Id);
            LimpiarUsuarioArea(message.AppUser.Id);
            dbUser.UserRegional = appUser.UserRegional;
            dbUser.UserArea = appUser.UserArea;

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

        private void LimpiarUsuarioRegional(int idUsuarioRegional)
        {
            var usersRegionales = usuarioRegionalRepository.Filter(new FindUserRegionalByUser(idUsuarioRegional));
            foreach (var regiones in usersRegionales) usuarioRegionalRepository.Delete(regiones);
        }
        private void LimpiarUsuarioArea(int idUsuarioArea)
        {
            var usuarioAreas = usuarioAreaRepository.Filter(new FindUserAreaByUser(idUsuarioArea));
            foreach (var areas in usuarioAreas) usuarioAreaRepository.Delete(areas);
        }
    }
}
