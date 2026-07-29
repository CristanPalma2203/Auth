using Application.Common;
using Domain.Repositories.Extenciones;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.AppUser
{
    public class ListUsers : QueryStringParameters, IAppMessage
    {
        public string Name { get; set; }
        public string correo { get; set; }
        public int idDepartamento { get; set; }
    }
}
