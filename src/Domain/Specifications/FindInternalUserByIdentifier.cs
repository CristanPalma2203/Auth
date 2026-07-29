using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindInternalUserByIdentifier : ISpecification<AppUser>
    {
        private readonly string correo;

        public FindInternalUserByIdentifier(string correo)
        {
            this.correo = correo;
        }
        public Func<AppUser, bool> Traer()
        {

            return new Func<AppUser, bool>(c => c.AccessIdentifier == correo && c.UserType==AppUser.internalUserType);
        }
    }
}
