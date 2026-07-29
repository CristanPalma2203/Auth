using Domain.Models;
using Domain.Repositories;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<RolePermission>, IRolePermissionRepository
    {
        public RoleRepository(AutenticationContext dbContext) : base(dbContext)
        {
        }
    }
}
