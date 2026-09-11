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

            // Compare AppUser.Password (EF → column Password). Never PasswordHash (column does not exist).
            // Original NRE at this file ~L28: AccessIdentifier.Replace or identifier.Replace when either was null.
            if (RegexUtilities.IsValidEmail(identifier))
            {
                return new Func<AppUser, bool>(c =>
                    AccessIdentifiersEqualEmail(c?.AccessIdentifier, identifier)
                    && StoredPasswordEquals(c?.Password, pass));
            }

            return new Func<AppUser, bool>(c =>
                AccessIdentifiersEqualDocument(c?.AccessIdentifier, identifier)
                && StoredPasswordEquals(c?.Password, pass));
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

        private static bool StoredPasswordEquals(string storedPassword, string incomingHash)
        {
            return !string.IsNullOrEmpty(storedPassword)
                && !string.IsNullOrEmpty(incomingHash)
                && storedPassword == incomingHash;
        }
    }
}
