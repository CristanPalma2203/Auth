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
            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrEmpty(password))
            {
                return _ => false;
            }

            var pass = AppUser.getPassword(password);
            if (string.IsNullOrEmpty(pass))
            {
                return _ => false;
            }

            if (RegexUtilities.IsValidEmail(identifier))
            {
                return new Func<AppUser, bool>(c =>
                    AccessIdentifiersEqualEmail(c?.AccessIdentifier, identifier)
                    && !string.IsNullOrEmpty(c?.Password)
                    && c.Password == pass);
            }

            return new Func<AppUser, bool>(c =>
                AccessIdentifiersEqualDocument(c?.AccessIdentifier, identifier)
                && !string.IsNullOrEmpty(c?.Password)
                && c.Password == pass);
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
