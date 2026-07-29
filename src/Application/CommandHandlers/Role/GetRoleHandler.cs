using Application.Commands.Role;
using Application.Dtos;
using MapsterMapper;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CommandHandlers.Role
{
    public class GetRoleHandler : AbstractHandler<GetRole>
    {
        private readonly IRoleRepository rolrepository;
        private readonly IMapper mapper;

        public GetRoleHandler(IRoleRepository rolrepository, IMapper mapper)
        {
            this.rolrepository = rolrepository;
            this.mapper = mapper;
        }
        public override IResponse Handle(GetRole message)
        {
            var Roles = rolrepository.GetByIdWithPermissions(message.id);
            return mapper.Map<RoleDto>(Roles);

        }
    }
}
