using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindExternalUserByEmailIdentifier : ISpecification<ExternalUser>
    {
        private readonly string correo;
        private readonly string identifier;

        public FindExternalUserByEmailIdentifier(string correo, string identifier)
        {
            this.correo = correo;
            this.identifier = identifier;
        }

        public Func<ExternalUser, bool> Traer()
        {
        return new Func<ExternalUser, bool>(c => c.Email.ToLower().Trim() == correo.ToLower().Trim() && c.Identifier.Replace("-", "").Trim() == identifier.Replace("-", "").Trim());

        }
    }
}
