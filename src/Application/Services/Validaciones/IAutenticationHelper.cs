using System.Collections.Generic;

namespace Application.Services.Validaciones
{
    public interface IAutenticationHelper
    {
        /// <summary>Valida token + al menos uno de los permisos. Lista vacía = público (no exige token).</summary>
        void Autenticado(IList<string> permisos);

        /// <summary>Exige JWT válido, sin permiso concreto.</summary>
        void RequireAuthenticated();
    }
}
