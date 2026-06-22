using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Importador
{
    public class RestablecerContrasenaImpotador: IAppMessage
    {
        public string Usuario { get; set; }
        public string Correo { get; set; }
    }
}
