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
            var lista = message.all == true && tenantContext.IsPlatformAdmin
                ? roleRepository.GetAll()
                : roleRepository.Filter(new FindRolesByTenant(tenantContext.TenantId, tenantContext.IsPlatformAdmin, soloAsignables: true));
            foreach (var item in lista) listaDto.Add(mapper.Map<RoleDto>(item));

            return new DtoListaRolesSinPaginar { Lista = listaDto };
        }
    }
}
