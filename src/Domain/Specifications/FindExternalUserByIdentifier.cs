using Domain.Models;
using System;

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
            var id = (identifier ?? "").Replace("-", "").Trim().ToLower();

            return c =>
            {
                if (c == null || string.IsNullOrEmpty(id)) return false;
                var cId = (c.Identifier ?? "").Replace("-", "").Trim().ToLower();
                var cMail = (c.Email ?? "").ToLower().Trim();
                return cId == id || cMail == id;
            };
        }
    }
}
