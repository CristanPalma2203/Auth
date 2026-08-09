using Application.Commands.Role;
using Application.Dtos;
using Application.Exceptions;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Service;
using MapsterMapper;
using System.Collections.Generic;
using System.Linq;

namespace Application.CommandHandlers.Role
{
    public class EditRoleHandler : AbstractHandler<EditRole>
    {
        private readonly IRoleRepository roleRepository;
        private readonly IMapper mapper;
        private readonly IRolePermissionRepository rolePermissionRepository;
        private readonly IEmailHelper correoHelper;
        private readonly ITokenService tokenService;
        private readonly IAppUserRepository appUserRepository;
        private readonly ITenantContext tenantContext;

        public EditRoleHandler(
            IRoleRepository roleRepository,
            IAppUserRepository appUserRepository,
            ITokenService tokenService,
            IEmailHelper correoHelper,
            IMapper mapper,
            IRolePermissionRepository rolePermissionRepository,
            ITenantContext tenantContext)
        {
            this.roleRepository = roleRepository;
            this.mapper = mapper;
            this.rolePermissionRepository = rolePermissionRepository;
            this.correoHelper = correoHelper;
            this.tokenService = tokenService;
            this.appUserRepository = appUserRepository;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(EditRole message)
        {
            var idUsuario = tokenService.GetUserId();
            var appUser = appUserRepository.GetByIdConRoles(idUsuario);
            var dbrol = roleRepository.GetByIdWithPermissions(message.Id);
            if (dbrol == null)
                throw new HttpException(404, "Role no encontrado");

            tenantContext.EnsureSameTenantOrPlatform(dbrol.TenantId);

            // No editar el rol con el que el usuario está autenticado
            if (appUser?.Roles != null && appUser.Roles.Any(r => r.RoleId == message.Id))
                throw new HttpException(403, "No puede editar el rol que tiene asignado. Solo puede verlo.");

            EnsureInheritablePermissions(message.Role.PermissionIds);

            foreach (var item in dbrol.Permissions) rolePermissionRepository.Delete(item.Id);
            dbrol.Update(message.Role.Name, message.Role.Description, message.Role.PermissionIds);
            var rolCreado = roleRepository.Update(message.Id, dbrol);
            correoHelper.SendRoleCreatedEmail(appUser.Name, message.Role.Name);
            return mapper.Map<RoleDto>(rolCreado);
        }

        private void EnsureInheritablePermissions(IList<int> permisoIds)
        {
            if (tenantContext.IsPlatformAdmin) return;
            if (permisoIds == null || permisoIds.Count == 0) return;
            var allowed = new HashSet<int>(tokenService.GetPermissions().Where(p => p != null).Select(p => p.Id));
            if (permisoIds.Any(id => !allowed.Contains(id)))
                throw new HttpException(403, "Solo puede asignar permisos que usted tiene");
        }
    }
}
