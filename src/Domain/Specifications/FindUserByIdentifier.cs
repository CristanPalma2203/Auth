using Domain.Models;
using Domain.Utilities;
using System;

namespace Domain.Specifications
{
    public class FindUserByIdentifier : ISpecification<AppUser>
    {
        private readonly string identifier;

        public FindUserByIdentifier(string identifier)
        {
            this.identifier = identifier;
        }
        public Func<AppUser, bool> Traer()
        {
            if (RegexUtilities.IsValidEmail(identifier))
            {
                return new Func<AppUser, bool>(c => c.AccessIdentifier.ToLower().Trim() == identifier.ToLower().Trim());
            }
            else {
                return new Func<AppUser, bool>(c => c.AccessIdentifier.Replace("-","").Trim() == identifier.Replace("-","").Trim());
            }
                
        }
    }
}
