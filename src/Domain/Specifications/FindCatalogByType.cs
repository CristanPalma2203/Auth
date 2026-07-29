using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindCatalogByType : ISpecification<Catalog>
    {
        private readonly string tipo;

        public FindCatalogByType(string tipo) {
            this.tipo = tipo;
        }
        public Func<Catalog, bool> Traer()
        {

            return new Func<Catalog, bool>(c => c.Type == tipo);
        }
    }
}
