using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Rol
{
    public class ConsultarRolesSinPaginar : IAppMessage
    {
        public bool all { get; set; }
    }
}
