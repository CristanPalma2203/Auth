using Domain.Models;
using Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindUserByIdentifierAndCode : ISpecification<AppUser>
    {
        private readonly string identifier;
        private readonly string codigo;

        public FindUserByIdentifierAndCode(string identifier, string codigo)
        {
            this.identifier = identifier;
            this.codigo = codigo;
        }
        public Func<AppUser, bool> Traer()
        {
            if (RegexUtilities.IsValidEmail(identifier))
            {
                return new Func<AppUser, bool>(c => c.AccessIdentifier.ToLower().Trim() == this.identifier.ToLower().Trim() && c.TemporaryCode == codigo);
            }
            else {
                return new Func<AppUser, bool>(c => c.AccessIdentifier.Replace("-", "").Trim() == this.identifier.Replace("-", "").Trim() && c.TemporaryCode == codigo);
            }
        
        }
    }
}
