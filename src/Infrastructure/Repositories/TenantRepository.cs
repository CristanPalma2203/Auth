using Domain.Models;
using Domain.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(AutenticationContext dbContext) : base(dbContext)
        {
        }
    }
}
