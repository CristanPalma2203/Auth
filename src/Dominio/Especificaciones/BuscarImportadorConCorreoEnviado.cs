using Dominio.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Especificaciones
{
   public class BuscarImportadorConCorreoEnviado : ISpecification<UsuarioExterno>
    {
        private readonly int id;

        public BuscarImportadorConCorreoEnviado(int id) {
            this.id = id;
        }
        public Func<UsuarioExterno, bool> Traer()
        {
            return new Func<UsuarioExterno, bool>(c => c.Id == id && c.CorreoEnviado==true);
        }
    }
}
