using Dominio.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Especificaciones
{
    public class BuscarImportadorPorIdentificadorYNombre : ISpecification<UsuarioExterno>

    {
        private readonly string identificador; 
        private readonly string nombre;
        public BuscarImportadorPorIdentificadorYNombre(string identificador, string nombre)
        {

            this.identificador = identificador;
            this.nombre = nombre;
        }

        public Func<UsuarioExterno, bool> Traer()
        {
            if (this.identificador != null && this.nombre != null)
            {
                return new Func<UsuarioExterno, bool>(c => c.Nombre.ToLower().Contains(nombre.ToLower()) && c.Identificador.Replace("-", "").Trim().Contains(identificador.Replace("-", "").Trim()));
            }
            else if (this.identificador != null)
            {
                return new Func<UsuarioExterno, bool>(c => c.Identificador.Replace("-", "").Trim().Contains(identificador.Replace("-", "").Trim()));
            }
            else
            {
                return new Func<UsuarioExterno, bool>(c => c.Nombre.ToLower().Contains(nombre.ToLower()));
            }

        }
    }
}
