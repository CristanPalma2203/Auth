using Domain.Models;
using Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindUserByIdentifierAndPassword : ISpecification<AppUser>
    {
        private readonly string identifier;
        private readonly string password;

        public FindUserByIdentifierAndPassword(string identifier, string password)
        {
            this.identifier = identifier;
            this.password = password;
        }
        public Func<AppUser, bool> Traer()
        {

            var pass = AppUser.getPassword(password);
            if (RegexUtilities.IsValidEmail(identifier))
            {
                return new Func<AppUser, bool>(c => c.AccessIdentifier.ToLower().Trim() == this.identifier.ToLower().Trim() && c.Password == pass);
            }
            else {
                return new Func<AppUser, bool>(c => c.AccessIdentifier.Replace("-", "").Trim() == this.identifier.Replace("-", "").Trim() && c.Password == pass);
            }
        
        }
    }
}
