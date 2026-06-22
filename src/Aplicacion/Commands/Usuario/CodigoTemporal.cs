using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Usuario
{
    public class CodigoTemporal : IAppMessage
    {
        public string IdentificadorAcceso { get; set; }
    }
}
