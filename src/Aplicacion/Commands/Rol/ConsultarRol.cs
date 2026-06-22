using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Rol
{
    public class ConsultarRol : IAppMessage
    {
        public int id { get; set; }
    }
}
