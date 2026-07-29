using Domain.Models;
using System;

namespace Domain.Specifications
{
    public class FindUserAreaByUser : ISpecification<UserArea>
    {
        private readonly int idUsuarioArea;
        public FindUserAreaByUser(int idUsuarioArea)
        {
            this.idUsuarioArea = idUsuarioArea;
        }
        public Func<UserArea, bool> Traer()
        {
            return new Func<UserArea, bool>(c => c.UserId == idUsuarioArea);
        }
    }
}
