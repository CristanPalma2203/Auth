using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindAssignableRoles : ISpecification<Role>
    {

        public FindAssignableRoles()
        {
        }
      

        Func<Role, bool> ISpecification<Role>.Traer()
        {
            return new Func<Role, bool>(c => c.IsAssignable == true);
        }
    }
}