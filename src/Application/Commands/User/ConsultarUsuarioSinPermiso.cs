using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.AppUser
{
    public class ListUsersWithoutPermission : IAppMessage
    {
        public int Id { get; set; }
    }
}
