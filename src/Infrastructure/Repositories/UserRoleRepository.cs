using Domain.Models;
using Domain.Repositories;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUsuarioRolRepository
    {
        public UserRoleRepository(AutenticationContext dbContext) : base(dbContext)
        {
        }
    }
}
