using Aplicacion.Commands.Rol;
using Aplicacion.Dtos;
using Dominio.Especificaciones;
using Dominio.Repositories;
using Dominio.Repositories.Extensiones;
using Dominio.Service;
using MapsterMapper;

namespace Aplicacion.CommandHandlers.Rol
{
    public class ConsultarRolesHandler : AbstractHandler<ConsultarRoles>
    {
        private readonly IRolRepository rolRepository;
        private readonly IMapper mapper;
        private readonly ITenantContext tenantContext;

        public ConsultarRolesHandler(IRolRepository rolRepository, IMapper mapper, ITenantContext tenantContext)
        {
            this.rolRepository = rolRepository;
            this.mapper = mapper;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(ConsultarRoles message)
        {
            var tenantSpec = new BuscarRolesPorTenant(tenantContext.TenantId, tenantContext.IsPlatformAdmin, soloAsignables: true);

            if (message.Name != null)
            {
                var nameSpec = new BuscarRolPorNombre(message.Name);
                var respuestaFiltrada = rolRepository.ConsultarPaginado(message, new SpecAndRol(nameSpec, tenantSpec));
                return mapper.Map<RolPaginado>(respuestaFiltrada);
            }

            var respuesta = rolRepository.ConsultarPaginado(message, tenantSpec);
            return mapper.Map<RolPaginado>(respuesta);
        }
    }

    internal class SpecAndRol : ISpecification<Dominio.Models.Rol>
    {
        private readonly ISpecification<Dominio.Models.Rol> a;
        private readonly ISpecification<Dominio.Models.Rol> b;

        public SpecAndRol(ISpecification<Dominio.Models.Rol> a, ISpecification<Dominio.Models.Rol> b)
        {
            this.a = a;
            this.b = b;
        }

        public System.Func<Dominio.Models.Rol, bool> Traer()
        {
            var fa = a.Traer();
            var fb = b.Traer();
            return x => fa(x) && fb(x);
        }
    }
}
