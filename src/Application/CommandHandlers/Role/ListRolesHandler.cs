using Application.Commands.Role;
using Application.Dtos;
using Domain.Specifications;
using Domain.Repositories;
using Domain.Repositories.Extensiones;
using Domain.Service;
using MapsterMapper;

namespace Application.CommandHandlers.Role
{
    public class ListRolesHandler : AbstractHandler<ListRoles>
    {
        private readonly IRoleRepository roleRepository;
        private readonly IMapper mapper;
        private readonly ITenantContext tenantContext;

        public ListRolesHandler(IRoleRepository roleRepository, IMapper mapper, ITenantContext tenantContext)
        {
            this.roleRepository = roleRepository;
            this.mapper = mapper;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(ListRoles message)
        {
            // Admin plataforma ve todos los roles (incl. Admin de plataforma).
            // Tenant: solo roles asignables de su empresa.
            var tenantSpec = new FindRolesByTenant(
                tenantContext.TenantId,
                tenantContext.IsPlatformAdmin,
                soloAsignables: !tenantContext.IsPlatformAdmin);

            if (message.Name != null)
            {
                var nameSpec = new FindRoleByName(message.Name);
                var respuestaFiltrada = roleRepository.GetPaged(message, new SpecAndRol(nameSpec, tenantSpec));
                return mapper.Map<RolesPagedDto>(respuestaFiltrada);
            }

            var respuesta = roleRepository.GetPaged(message, tenantSpec);
            return mapper.Map<RolesPagedDto>(respuesta);
        }
    }

    internal class SpecAndRol : ISpecification<Domain.Models.Role>
    {
        private readonly ISpecification<Domain.Models.Role> a;
        private readonly ISpecification<Domain.Models.Role> b;

        public SpecAndRol(ISpecification<Domain.Models.Role> a, ISpecification<Domain.Models.Role> b)
        {
            this.a = a;
            this.b = b;
        }

        public System.Func<Domain.Models.Role, bool> Traer()
        {
            var fa = a.Traer();
            var fb = b.Traer();
            return x => fa(x) && fb(x);
        }
    }
}
