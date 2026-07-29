using Domain.Models;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Domain.Specifications
{
    public class FindExternalUserByVerificationToken : ISpecification<ExternalUser>
    {
        private readonly string token;

        public FindExternalUserByVerificationToken(string token)
        {
            this.token = token;
        }

        public Func<ExternalUser, bool> Traer()
        {
            return new Func<ExternalUser, bool>(c => c.VerificationToken == token);

        }
    }
}
