using Application.Common;
using Domain.Repositories.Extenciones;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Role
{
    public class ListRoles : QueryStringParameters, IAppMessage
    {
        public string Name { get; set; }
    }
}
