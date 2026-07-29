using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class RoleModules : ISpecification<Role>
    {

        public RoleModules()
        {
        }

        public Func<Role, bool> Traer()
        {
            var accesos = new List<int> { Role.IdRolUsuarioRecibo };
            return new Func<Role, bool>(c => accesos.Contains( c.Id ));
        }

       
    }
}