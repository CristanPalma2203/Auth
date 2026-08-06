using Domain.Models;
using System;

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
            var mail = (correo ?? "").ToLower().Trim();
            var id = (identifier ?? "").Replace("-", "").Trim().ToLower();

            return c =>
            {
                if (c == null) return false;
                var cMail = (c.Email ?? "").ToLower().Trim();
                var cId = (c.Identifier ?? "").Replace("-", "").Trim().ToLower();
                // Compradores web: Identifier suele ser el mismo correo
                return cMail == mail && (cId == id || cMail == id);
            };
        }
    }
}
