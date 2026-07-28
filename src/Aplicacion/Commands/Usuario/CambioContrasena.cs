using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Usuario
{
   public class CambioContrasena: IAppMessage
    {
 
        public string Password { get; set; }
        public string AccessIdentifier { get; set; }
        public int Id { get; set; }

    }
}
