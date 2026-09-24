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

        public CreateRoleHandler(
            IRoleRepository roleRepository,
            ITenantRepository tenantRepository,
            IMapper mapper,
            IEmailHelper correoHelper,
            ITokenService tokenService,
            IAppUserRepository appUserRepository,
            ITenantContext tenantContext)
        {
            this.roleRepository = roleRepository;
            this.tenantRepository = tenantRepository;
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

            var tenantId = tenantContext.IsPlatformAdmin ? message.Role.TenantId : tenantContext.TenantId;
            if (!tenantId.HasValue)
                throw new HttpException(422, "Debe seleccionar la empresa del rol");
            var tenant = tenantRepository.GetById(tenantId.Value);
            if (tenant == null || !tenant.IsActive)
                throw new HttpException(422, "La empresa seleccionada no existe o está inactiva");

            var permissionIds = DistinctPermissionIds(message.Role?.PermissionIds);
            EnsureInheritablePermissions(permissionIds);

            var Roles = mapper.Map<Domain.Models.Role>(message.Role);
            Roles.SetCreatedAt();
            Roles.CreateRolePermissions(permissionIds);
            Roles.IsAssignable = true;
            Roles.TenantId = tenantId.Value;
            var rolCreado = roleRepository.Create(Roles);
            try
            {
                correoHelper.SendRoleCreatedEmail(appUser?.Name, Roles.Name);
            }
            catch (System.Exception)
            {
                // El correo no debe impedir crear el rol.
            }
            return mapper.Map<RoleDto>(rolCreado);
        }

        private static IList<int> DistinctPermissionIds(IList<int> permisoIds)
        {
            if (permisoIds == null) return new List<int>();
            return permisoIds.Where(id => id > 0).Distinct().ToList();
        }

        private void EnsureInheritablePermissions(IList<int> permisoIds)
        {
            if (permisoIds == null || permisoIds.Count == 0) return;

            // Platform admin may assign any permissionId to a new tenant role, even when
            // the tenant has not contracted that module. A null TenantId never reaches
            // this check: Handle still requires an active company.
            if (tenantContext.IsPlatformAdmin)
                return;

            var allowed = new HashSet<int>(
                tokenService.GetPermissions().Where(p => p != null).Select(p => p.Id));
            if (permisoIds.Any(id => !allowed.Contains(id)))
                throw new HttpException(403, "Solo puede asignar permisos de los módulos contratados / que usted tiene");
        }
    }
}
