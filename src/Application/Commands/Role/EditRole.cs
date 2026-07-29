using Application.Common;
using Application.Dtos;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Role
{
    public class EditRole : IAppMessage
    {
        public RoleDto Role { get; set; }
        public int Id { get; set; }
    }
}
