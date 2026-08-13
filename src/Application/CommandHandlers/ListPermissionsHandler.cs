using Application.Commands;
using Application.Dtos;
using Application.Services.PermissionQuery;
using Domain.Specifications;
using Domain.Models;
using Domain.Repositories;
using Domain.Service;
using MapsterMapper;
using System.Collections.Generic;
using System.Linq;

namespace Application.CommandHandlers
{
    public class ListPermissionsHandler : AbstractHandler<ListPermissions>
    {
        private readonly IMapper mapper;
        private readonly IPermissionRepository permissionsRepo;
        private readonly IPermissionQueryService consultarPermisoService;
        private readonly ITokenService tokenService;
        private readonly ITenantContext tenantContext;
        private readonly ITenantContractPermissionService contractPermissions;

        public ListPermissionsHandler(
            IMapper mapper,
            IPermissionRepository permisorepo,
            IPermissionQueryService consultarPermisoService,
            ITokenService tokenService,
            ITenantContext tenantContext,
            ITenantContractPermissionService contractPermissions)
        {
            this.mapper = mapper;
            this.permissionsRepo = permisorepo;
            this.consultarPermisoService = consultarPermisoService;
            this.tokenService = tokenService;
            this.tenantContext = tenantContext;
            this.contractPermissions = contractPermissions;
        }

        public override IResponse Handle(ListPermissions message)
        {
            var permisos = permissionsRepo.Filter(new FindAssignablePermissions()).ToList();

            if (!tenantContext.IsPlatformAdmin)
            {
                var allowedIds = new HashSet<int>(
                    tokenService.GetPermissions()
                        .Where(p => p != null)
                        .Select(p => p.Id));
                permisos = permisos.Where(p => allowedIds.Contains(p.Id)).ToList();
            }
            else if (message.TenantId.HasValue && message.TenantId.Value > 0)
            {
                var allowed = contractPermissions.AllowedPermissionIds(message.TenantId);
                if (allowed != null)
                    permisos = permisos.Where(p => allowed.Contains(p.Id)).ToList();
            }

            IList<PermissionDto> permissionDtos = new List<PermissionDto>();
            foreach (var item in permisos)
                permissionDtos.Add(mapper.Map<PermissionDto>(item));
            return new PermissionsResponse { Permissions = consultarPermisoService.Estructurar(permissionDtos) };
        }
    }
}
