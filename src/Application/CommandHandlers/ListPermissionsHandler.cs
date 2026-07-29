using Application.Commands;
using Application.Dtos;
using Application.Exceptions;
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

        public ListPermissionsHandler(
            IMapper mapper,
            IPermissionRepository permisorepo,
            IPermissionQueryService consultarPermisoService,
            ITokenService tokenService,
            ITenantContext tenantContext)
        {
            this.mapper = mapper;
            this.permissionsRepo = permisorepo;
            this.consultarPermisoService = consultarPermisoService;
            this.tokenService = tokenService;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(ListPermissions message)
        {
            var permisos = permissionsRepo.Filter(new FindAssignablePermissions()).ToList();

            // Herencia: Tenants admin solo ve/asigna los permisos que él tiene.
            if (!tenantContext.IsPlatformAdmin)
            {
                var allowedIds = new HashSet<int>(
                    tokenService.GetPermissions()
                        .Where(p => p != null)
                        .Select(p => p.Id));
                permisos = permisos.Where(p => allowedIds.Contains(p.Id)).ToList();
            }

            IList<PermissionDto> permissionDtos = new List<PermissionDto>();
            foreach (var item in permisos)
                permissionDtos.Add(mapper.Map<PermissionDto>(item));
            return new PermissionsResponse { Permissions = consultarPermisoService.Estructurar(permissionDtos) };
        }
    }
}
