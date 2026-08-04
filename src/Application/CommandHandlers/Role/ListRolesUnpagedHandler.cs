using Application.Commands.Role;
using Application.Dtos;
using Domain.Specifications;
using Domain.Repositories;
using Domain.Service;
using MapsterMapper;
using System.Collections.Generic;

namespace Application.CommandHandlers.Role
{
    public class ListRolesUnpagedHandler : AbstractHandler<ListRolesUnpaged>
    {
        private readonly IRoleRepository roleRepository;
        private readonly IMapper mapper;
        private readonly ITenantContext tenantContext;

        public ListRolesUnpagedHandler(
            IRoleRepository roleRepository,
            IMapper mapper,
            ITenantContext tenantContext)
        {
            this.roleRepository = roleRepository;
            this.mapper = mapper;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(ListRolesUnpaged message)
        {
            var listaDto = new List<RoleDto>();

            // El admin de plataforma puede pedir los roles de una empresa concreta; el resto
            // siempre queda acotado a la suya, sin importar lo que envie.
            var filtrarPorTenant = tenantContext.IsPlatformAdmin && message.tenantId.HasValue;

            var lista = message.all == true && tenantContext.IsPlatformAdmin && !filtrarPorTenant
                ? roleRepository.GetAll()
                : roleRepository.Filter(filtrarPorTenant
                    ? new FindRolesByTenant(message.tenantId, isPlatformAdmin: false, soloAsignables: true)
                    : new FindRolesByTenant(tenantContext.TenantId, tenantContext.IsPlatformAdmin, soloAsignables: true));
            foreach (var item in lista) listaDto.Add(mapper.Map<RoleDto>(item));

            return new DtoListaRolesSinPaginar { Lista = listaDto };
        }
    }
}
