using Dominio.Models;
using Dominio.Repositories.Extenciones;
using Dominio.Repositories.Extensiones;

namespace Dominio.Repositories
{
   public interface IUsuarioExternoRepository : IGenericRepository<UsuarioExterno>
    {
        UsuarioExterno GetByIdConDependencias(int id);
        IPagina<UsuarioExterno> Filter(IConsulta ownerParameters, string especificaciones);
    }
}
