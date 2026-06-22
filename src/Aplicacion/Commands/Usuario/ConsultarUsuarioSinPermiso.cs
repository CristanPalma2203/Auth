using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Usuario
{
    public class ConsultarUsuarioSinPermiso : IAppMessage
    {
        public int Id { get; set; }
    }
}
