using Domain.Specifications;
using Domain.Models;
using Domain.Repositories.Extenciones;
using Domain.Repositories.Extensiones;

namespace Domain.Repositories
{
    public interface IRoleRepository:IGenericRepository<Role>
    {
        Role GetByIdWithPermissions(int id);
    }
}
