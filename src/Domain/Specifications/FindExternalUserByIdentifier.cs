using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindExternalUserByIdentifier : ISpecification<ExternalUser>
    {
      
        private readonly string identifier;

        public FindExternalUserByIdentifier(string identifier)
        {
           
            this.identifier = identifier;
        }

        public Func<ExternalUser, bool> Traer()
        {
      
            return new Func<ExternalUser, bool>(c =>  c.Identifier.Replace("-", "").Trim() == identifier.Replace("-", "").Trim());

        }
    }
}
