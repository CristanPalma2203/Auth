using Aplicacion.Commands.Rol;
using Aplicacion.Dtos;
using Dominio.Especificaciones;
using Dominio.Repositories;
using Dominio.Service;
using MapsterMapper;
using System.Collections.Generic;

namespace Aplicacion.CommandHandlers.Rol
{
    public class ConsultarRolesSinPaginarHandler : AbstractHandler<ConsultarRolesSinPaginar>
    {
        private readonly IRolRepository rolRepository;
        private readonly IMapper mapper;
        private readonly ITenantContext tenantContext;

        public ConsultarRolesSinPaginarHandler(
            IRolRepository rolRepository,
            IMapper mapper,
            ITenantContext tenantContext)
        {
            this.rolRepository = rolRepository;
            this.mapper = mapper;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(ConsultarRolesSinPaginar message)
        {
            var listaDto = new List<DtoRol>();
            var lista = message.all == true && tenantContext.IsPlatformAdmin
                ? rolRepository.GetAll()
                : rolRepository.Filter(new BuscarRolesPorTenant(tenantContext.TenantId, tenantContext.IsPlatformAdmin, soloAsignables: true));
            foreach (var item in lista) listaDto.Add(mapper.Map<DtoRol>(item));

            return new DtoListaRolesSinPaginar { Lista = listaDto };
        }
    }
}
