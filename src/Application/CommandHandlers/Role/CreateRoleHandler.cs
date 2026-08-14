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
        private readonly ITenantRepository tenantRepository;
        private readonly IMapper mapper;
        private readonly IEmailHelper correoHelper;
        private readonly ITokenService tokenService;
        private readonly IAppUserRepository appUserRepository;
        private readonly ITenantContext tenantContext;
        private readonly ITenantContractPermissionService contractPermissions;

        public CreateRoleHandler(
            IRoleRepository roleRepository,
            ITenantRepository tenantRepository,
            IMapper mapper,
            IEmailHelper correoHelper,
            ITokenService tokenService,
            IAppUserRepository appUserRepository,
            ITenantContext tenantContext,
            ITenantContractPermissionService contractPermissions)
        {
            this.roleRepository = roleRepository;
            this.tenantRepository = tenantRepository;
            this.mapper = mapper;
            this.correoHelper = correoHelper;
            this.tokenService = tokenService;
            this.appUserRepository = appUserRepository;
            this.tenantContext = tenantContext;
            this.contractPermissions = contractPermissions;
        }

        public override IResponse Handle(CreateRole message)
        {
            var idUsuario = tokenService.GetUserId();
            var appUser = appUserRepository.GetById(idUsuario);

            var targetTenantId = tenantContext.IsPlatformAdmin
                ? (int?)null
                : tenantContext.TenantId;

            EnsureInheritablePermissions(message.Role.PermissionIds, targetTenantId);

            var tenantId = tenantContext.IsPlatformAdmin ? message.Role.TenantId : tenantContext.TenantId;
            if (!tenantId.HasValue)
                throw new HttpException(422, "Debe seleccionar la empresa del rol");
            var tenant = tenantRepository.GetById(tenantId.Value);
            if (tenant == null || !tenant.IsActive)
                throw new HttpException(422, "La empresa seleccionada no existe o está inactiva");

            var Roles = mapper.Map<Domain.Models.Role>(message.Role);
            Roles.SetCreatedAt();
            Roles.CreateRolePermissions(message.Role.PermissionIds);
            Roles.IsAssignable = true;
            Roles.TenantId = tenantId.Value;
            var rolCreado = roleRepository.Create(Roles);
            correoHelper.SendRoleCreatedEmail(appUser.Name, Roles.Name);
            return mapper.Map<RoleDto>(rolCreado);
        }

        private void EnsureInheritablePermissions(IList<int> permisoIds, int? roleTenantId)
        {
            if (permisoIds == null || permisoIds.Count == 0) return;

            HashSet<int> allowed = null;
            if (!tenantContext.IsPlatformAdmin)
            {
                allowed = new HashSet<int>(
                    tokenService.GetPermissions().Where(p => p != null).Select(p => p.Id));
            }
            else if (roleTenantId.HasValue)
            {
                allowed = contractPermissions.AllowedPermissionIds(roleTenantId);
            }

            if (allowed == null) return;
            if (permisoIds.Any(id => !allowed.Contains(id)))
                throw new HttpException(403, "Solo puede asignar permisos de los módulos contratados / que usted tiene");
        }
    }
}
