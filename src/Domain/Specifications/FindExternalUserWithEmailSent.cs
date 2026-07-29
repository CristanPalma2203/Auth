using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
   public class FindExternalUserWithEmailSent : ISpecification<ExternalUser>
    {
        private readonly int id;

        public FindExternalUserWithEmailSent(int id) {
            this.id = id;
        }
        public Func<ExternalUser, bool> Traer()
        {
            return new Func<ExternalUser, bool>(c => c.Id == id && c.EmailSent==true);
        }
    }
}
