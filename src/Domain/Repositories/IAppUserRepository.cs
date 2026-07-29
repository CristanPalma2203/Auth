using Domain.Specifications;
using Domain.Models;
using Domain.Repositories.Extenciones;
using Domain.Repositories.Extensiones;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repositories
{
    public interface IAppUserRepository:IGenericRepository<AppUser>
    {
        IPagina<AppUser> GetPagedWithRole(IConsulta ownerParameters, ISpecification<AppUser> busqueda);
        IPagina<AppUser> GetPagedWithRole(IConsulta ownerParameters);
        AppUser GetByIdConRoles(int id);

        AppUser GetUserWithRolePermissions(ISpecification<AppUser> busqueda);
        
    }
}
