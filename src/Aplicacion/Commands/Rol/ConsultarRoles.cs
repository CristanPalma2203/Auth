using Aplicacion.Common;
using Dominio.Repositories.Extenciones;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Rol
{
    public class ConsultarRoles : QueryStringParameters, IAppMessage
    {
        public string Name { get; set; }
    }
}
