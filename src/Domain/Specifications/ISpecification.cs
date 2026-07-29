using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public interface ISpecification<Entidad>
    {
        Func<Entidad, bool> Traer();
    }
}
