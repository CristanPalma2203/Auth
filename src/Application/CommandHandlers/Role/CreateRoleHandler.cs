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
    public class CreateRoleHandler : AbstractHandler<CreateRole>
    {
        private readonly IRoleRepository roleRepository;
        private readonly IMapper mapper;
        private readonly IEmailHelper correoHelper;
        private readonly ITokenService tokenService;
        private readonly IAppUserRepository appUserRepository;
        private readonly ITenantContext tenantContext;

        public CreateRoleHandler(
            IRoleRepository roleRepository,
            IMapper mapper,
            IEmailHelper correoHelper,
            ITokenService tokenService,
            IAppUserRepository appUserRepository,
            ITenantContext tenantContext)
        {
            this.roleRepository = roleRepository;
            this.mapper = mapper;
            this.correoHelper = correoHelper;
            this.tokenService = tokenService;
            this.appUserRepository = appUserRepository;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(CreateRole message)
        {
            var idUsuario = tokenService.GetUserId();
            var appUser = appUserRepository.GetById(idUsuario);

            EnsureInheritablePermissions(message.Role.PermissionIds);

            var Roles = mapper.Map<Domain.Models.Role>(message.Role);
            Roles.SetCreatedAt();
            Roles.CreateRolePermissions(message.Role.PermissionIds);
            Roles.IsAssignable = true;
            if (!tenantContext.IsPlatformAdmin)
                Roles.TenantId = tenantContext.TenantId;
            var rolCreado = roleRepository.Create(Roles);
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
