using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindRoleByName : ISpecification<Role>
    {
        private readonly string nombre;

        public FindRoleByName(string nombre)
        {
            this.nombre = nombre;
        }
      

        Func<Role, bool> ISpecification<Role>.Traer()
        {
            return new Func<Role, bool>(c => c.Name.ToLower().Contains(nombre.ToLower()) && c.IsAssignable == true);
        }
    }
}