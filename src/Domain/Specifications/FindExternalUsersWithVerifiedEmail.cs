using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindExternalUsersWithVerifiedEmail : ISpecification<ExternalUser>
    {
        public Func<ExternalUser, bool> Traer()
        {
            return new Func<ExternalUser, bool>(c => c.EmailVerified==true && c.AccessApprovedAt==null );
        }
    }
}
