using Aplicacion.Common;
using Aplicacion.Dtos;
using Aplicacion.Dtos.Usuario;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Usuario
{
    public class EditarUsuario : IAppMessage
    {
        public DtoUsuario Usuario { get; set; }
    }
}
