using Domain.Models;
using Domain.Repositories;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class UserAreaRepository : GenericRepository<UserArea>, IUsuarioAreaRepository
    {
        public UserAreaRepository(AutenticationContext dbContext) : base(dbContext)
        {
        }
    }
}
