using Aplicacion.Commands;
using Aplicacion.Dtos;
using Aplicacion.Exceptions;
using Aplicacion.Services.ConsultaPermiso;
using Dominio.Especificaciones;
using Dominio.Models;
using Dominio.Repositories;
using Dominio.Service;
using MapsterMapper;
using System.Collections.Generic;
using System.Linq;

namespace Aplicacion.CommandHandlers
{
    public class ConsultarPermisosHandler : AbstractHandler<ConsultarPermisos>
    {
        private readonly IMapper mapper;
        private readonly IPermisoRepository permisosRepo;
        private readonly IConsultaPermisoService consultarPermisoService;
        private readonly ITokenService tokenService;
        private readonly ITenantContext tenantContext;

        public ConsultarPermisosHandler(
            IMapper mapper,
            IPermisoRepository permisorepo,
            IConsultaPermisoService consultarPermisoService,
            ITokenService tokenService,
            ITenantContext tenantContext)
        {
            this.mapper = mapper;
            this.permisosRepo = permisorepo;
            this.consultarPermisoService = consultarPermisoService;
            this.tokenService = tokenService;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(ConsultarPermisos message)
        {
            var permisos = permisosRepo.Filter(new BuscarPermisosAsignables()).ToList();

            // Herencia: tenant admin solo ve/asigna los permisos que él tiene.
            if (!tenantContext.IsPlatformAdmin)
            {
                var allowedIds = new HashSet<int>(
                    tokenService.TraerPermisos()
                        .Where(p => p != null)
                        .Select(p => p.Id));
                permisos = permisos.Where(p => allowedIds.Contains(p.Id)).ToList();
            }

            IList<DtoPermiso> dtoPermiso = new List<DtoPermiso>();
            foreach (var item in permisos)
                dtoPermiso.Add(mapper.Map<DtoPermiso>(item));
            return new DtoPermisos { Permisos = consultarPermisoService.Estructurar(dtoPermiso) };
        }
    }
}
