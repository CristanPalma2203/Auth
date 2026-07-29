using Domain.Models;
using Domain.Repositories;
using Domain.Repositories.Extenciones;
using Domain.Repositories.Extensiones;
using Infrastructure.Data;
using Infrastructure.Repositories.Extenciones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Repositories
{
    public class ExternalUserRepository : GenericRepository<ExternalUser>, IExternalUserRepository
    {
        private readonly AutenticationContext dbContext;

        public ExternalUserRepository(AutenticationContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IPagina<ExternalUser> Filter(IConsulta ownerParameters, string especificaciones)
        {
            return PagedList<ExternalUser>.ToPagedList(dbContext.Set<ExternalUser>()
                  .Where(especificaciones),
                      ownerParameters.PageNumber,
                      ownerParameters.PageSize);
        }
        public ExternalUser GetByIdConDependencias(int id)
        {
            return dbContext.Set<ExternalUser>().AsNoTracking().
                Include(c=>c.Department).Include(c=>c.Municipality).Include(c=>c.Nationality).
                FirstOrDefault("Id="+id);
        }
    }
}
