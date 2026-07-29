using Domain.Models;
using System;

namespace Domain.Specifications
{
    public class FindUserById : ISpecification<AppUser>
    {
        private readonly int id;

        public FindUserById(int  id)
        {
            this.id = id;
        }
        public Func<AppUser, bool> Traer()
        {

            return new Func<AppUser, bool>(c => c.Id == id);
        }
    }
}
