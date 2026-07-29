using Domain.Models;
using Domain.Repositories.Extenciones;
using Domain.Repositories.Extensiones;

namespace Domain.Repositories
{
   public interface IExternalUserRepository : IGenericRepository<ExternalUser>
    {
        ExternalUser GetByIdConDependencias(int id);
        IPagina<ExternalUser> Filter(IConsulta ownerParameters, string especificaciones);
    }
}
