using Domain.Models;
using System;

namespace Domain.Specifications
{
    public class FindCatalogByTypeAndParent : ISpecification<Catalog>
    {
        private readonly string tipo;
        private readonly int idPadre;

        public FindCatalogByTypeAndParent(string tipo, int idPadre) {
            this.tipo = tipo;
            this.idPadre = idPadre;
        }
        public Func<Catalog, bool> Traer()
        {
            return new Func<Catalog, bool>(c => c.Type == tipo && c.ParentId == idPadre);
        }
    }
}
