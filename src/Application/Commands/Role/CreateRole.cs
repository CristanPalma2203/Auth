using Application.Common;
using Application.Dtos;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Role
{
    public class CreateRole : IAppMessage
    {
        public RoleDto Role { get; set; }
    }
}
