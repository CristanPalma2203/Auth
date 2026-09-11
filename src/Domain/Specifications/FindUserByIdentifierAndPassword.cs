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
            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrEmpty(codigo))
            {
                return _ => false;
            }

            if (RegexUtilities.IsValidEmail(identifier))
            {
                return new Func<AppUser, bool>(c =>
                    AccessIdentifiersEqualEmail(c?.AccessIdentifier, identifier)
                    && !string.IsNullOrEmpty(c?.TemporaryCode)
                    && c.TemporaryCode == codigo);
            }

            return new Func<AppUser, bool>(c =>
                AccessIdentifiersEqualDocument(c?.AccessIdentifier, identifier)
                && !string.IsNullOrEmpty(c?.TemporaryCode)
                && c.TemporaryCode == codigo);
        }

        private static bool AccessIdentifiersEqualEmail(string stored, string incoming)
        {
            if (string.IsNullOrWhiteSpace(stored) || string.IsNullOrWhiteSpace(incoming))
            {
                return false;
            }

            return stored.ToLower().Trim() == incoming.ToLower().Trim();
        }

        private static bool AccessIdentifiersEqualDocument(string stored, string incoming)
        {
            if (string.IsNullOrWhiteSpace(stored) || string.IsNullOrWhiteSpace(incoming))
            {
                return false;
            }

            return stored.Replace("-", "").Trim() == incoming.Replace("-", "").Trim();
        }
    }
}
