using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Validaciones
{
    public interface IAutenticationHelper
    {
         void Autenticado(IList<string> permisos);
    }
}
