using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindExternalUserByIdentifierAndName : ISpecification<ExternalUser>

    {
        private readonly string identifier; 
        private readonly string nombre;
        public FindExternalUserByIdentifierAndName(string identifier, string nombre)
        {

            this.identifier = identifier;
            this.nombre = nombre;
        }

        public Func<ExternalUser, bool> Traer()
        {
            if (this.identifier != null && this.nombre != null)
            {
                return new Func<ExternalUser, bool>(c => c.Name.ToLower().Contains(nombre.ToLower()) && c.Identifier.Replace("-", "").Trim().Contains(identifier.Replace("-", "").Trim()));
            }
            else if (this.identifier != null)
            {
                return new Func<ExternalUser, bool>(c => c.Identifier.Replace("-", "").Trim().Contains(identifier.Replace("-", "").Trim()));
            }
            else
            {
                return new Func<ExternalUser, bool>(c => c.Name.ToLower().Contains(nombre.ToLower()));
            }

        }
    }
}
