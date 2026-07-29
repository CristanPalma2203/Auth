using Domain.Specifications;
using Domain.Models;
using Domain.Repositories;
using Domain.Repositories.Extenciones;
using Domain.Repositories.Extensiones;
using Infrastructure.Data;
using Infrastructure.Repositories.Extenciones;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class AppUserRepository : GenericRepository<AppUser>, IAppUserRepository
    {
        private readonly AutenticationContext dbContext;

        public AppUserRepository(AutenticationContext dbContext)
        : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IPagina<AppUser> GetPagedWithRole(IConsulta ownerParameters, ISpecification<AppUser> busqueda)
        {
            return PagedList<AppUser>.ToPagedList(dbContext.Set<AppUser>().OrderBy(on => on.Id).Include("Roles.Role").Include(c => c.Department).Where(busqueda.Traer()).AsQueryable(),
                    ownerParameters.PageNumber,
                    ownerParameters.PageSize);
        }

        public IPagina<AppUser> GetPagedWithRole(IConsulta ownerParameters)
        {
            var q = dbContext.Set<AppUser>().OrderBy(on => on.Id)
                .Include("Roles.Role")
                .Include(c => c.Department)
                .Where(c => c.UserType == AppUser.internalUserType);
            return PagedList<AppUser>.ToPagedList(q,
                        ownerParameters.PageNumber,
                        ownerParameters.PageSize);
        }

        public AppUser GetByIdConRoles(int id)
        {
            return dbContext.Set<AppUser>().AsNoTracking()
                .Include("Roles.Role")
                .Include(c => c.UserRegional)
                .Include(c => c.UserArea)
                .Include(c => c.Tenant)
                .FirstOrDefault(e => e.Id == id);
        }

        public AppUser GetUserWithRolePermissions(ISpecification<AppUser> busqueda)
        {
            return dbContext.Set<AppUser>().AsNoTracking()
                .Include("Roles.Role.Permissions.Permission")
                .Include(c => c.Department)
                .Include(c => c.Tenant)
                .Include(c => c.UserArea)
                .Include(c => c.UserRegional)
                .FirstOrDefault(busqueda.Traer());
        }
    }
}
