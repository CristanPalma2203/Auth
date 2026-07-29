using Domain.Specifications;
using Domain.Models;
using Domain.Repositories;
using Domain.Repositories.Extenciones;
using Domain.Repositories.Extensiones;
using Infrastructure.Data;
using Infrastructure.Repositories.Extenciones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Repositories
{
    public class RolePermissionRepository : GenericRepository<Role>, IRoleRepository
    {
        private readonly AutenticationContext dbContext;

        public RolePermissionRepository(AutenticationContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

      

        public Role GetByIdWithPermissions(int id)
        {
            return dbContext.Set<Role>().AsNoTracking().Include(c => c.Permissions).FirstOrDefault(e => e.Id == id); ;
        }
    }
}
