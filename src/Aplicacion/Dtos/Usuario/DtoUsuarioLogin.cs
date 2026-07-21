using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Dtos.Usuario
{
    public class DtoUsuarioLogin: DtoUsuarioBase, IResponse
    {
        public string Token { get; set; }

    }
}
