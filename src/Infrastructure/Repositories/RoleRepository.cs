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
            // Tracked: el update del rol muta esta instancia. AsNoTracking + Update()
            // chocaba con Delete de RolePermission (misma key, dos instancias) → 500.
            return dbContext.Set<Role>().Include(c => c.Permissions).FirstOrDefault(e => e.Id == id);
        }
    }
}
