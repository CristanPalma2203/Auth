using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Usuario
{
   public class CambioContrasena: IAppMessage
    {
 
        public string Contrasena { get; set; }
        public string IdentificadorAcceso { get; set; }
        public int Id { get; set; }

    }
}
