using Dominio.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Especificaciones
{
    public class BuscarImportadoresConCorreoVerificado : ISpecification<UsuarioExterno>
    {
        public Func<UsuarioExterno, bool> Traer()
        {
            return new Func<UsuarioExterno, bool>(c => c.CorreoVerificado==true && c.FechaAprobacionAcceso==null );
        }
    }
}
