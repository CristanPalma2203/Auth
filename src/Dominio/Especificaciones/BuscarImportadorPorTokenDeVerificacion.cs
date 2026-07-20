using Dominio.Models;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Dominio.Especificaciones
{
    public class BuscarImportadorPorTokenDeVerificacion : ISpecification<UsuarioExterno>
    {
        private readonly string token;

        public BuscarImportadorPorTokenDeVerificacion(string token)
        {
            this.token = token;
        }

        public Func<UsuarioExterno, bool> Traer()
        {
            return new Func<UsuarioExterno, bool>(c => c.TokenVerificacion == token);

        }
    }
}
