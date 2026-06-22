using Aplicacion.Common;
using Aplicacion.Dtos.Usuario;
using Aplicacion.Services;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Usuario
{
    public class RegistrarUsuario : IAppMessage
    {
        public DtoUsuario Usuario { get; set; }
    }
}
