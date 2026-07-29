using Domain.Models;
using System;

namespace Domain.Specifications
{
    public class FindUserRegionalByUser : ISpecification<UserRegional>
    {
        private readonly int idUsuarioRegional;
        public FindUserRegionalByUser(int idUsuarioRegional)
        {
            this.idUsuarioRegional = idUsuarioRegional;
        }
        public Func<UserRegional, bool> Traer()
        {
            return new Func<UserRegional, bool>(c => c.UserId == idUsuarioRegional);
        }
    }
}
