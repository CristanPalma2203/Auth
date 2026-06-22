using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Importador
{
    public class VerificarCorreo: IAppMessage
    {
        public string Token { get; set; }
    }
}
