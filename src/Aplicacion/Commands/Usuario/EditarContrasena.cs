using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Usuario
{
    public class EditarContrasena : IAppMessage
    {

        public string Contrasena { get; set; }
        public int Id { get; set; }

    }
}
