using Dominio.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Especificaciones
{
    public class BuscarRolPorNombre : ISpecification<Rol>
    {
        private readonly string nombre;

        public BuscarRolPorNombre(string nombre)
        {
            this.nombre = nombre;
        }
      

        Func<Rol, bool> ISpecification<Rol>.Traer()
        {
            return new Func<Rol, bool>(c => c.Name.ToLower().Contains(nombre.ToLower()) && c.IsAssignable == true);
        }
    }
}