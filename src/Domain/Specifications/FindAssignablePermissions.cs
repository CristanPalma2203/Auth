using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindAssignablePermissions : ISpecification<Permission>
    {
        
        public FindAssignablePermissions()
        {
        }
      

        Func<Permission, bool> ISpecification<Permission>.Traer()
        {
            return new Func<Permission, bool>(c => c.IsAssignable == true);
        }
    }
}