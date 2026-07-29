using Domain.Models;
using Domain.Repositories;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class UserRegionalRepository : GenericRepository<UserRegional>, IUsuarioRegionalRepository
    {
        public UserRegionalRepository(AutenticationContext dbContext) : base(dbContext)
        {
        }
    }
}
