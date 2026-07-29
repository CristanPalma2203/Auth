using Domain.Models;
using System;

namespace Domain.Specifications
{
    public class FindUsersByTenant : ISpecification<AppUser>
    {
        private readonly int? tenantId;
        private readonly bool platformSeesAll;

        public FindUsersByTenant(int? actorTenantId, bool isPlatformAdmin)
        {
            tenantId = actorTenantId;
            platformSeesAll = isPlatformAdmin;
        }

        public Func<AppUser, bool> Traer()
        {
            if (platformSeesAll)
                return c => c.UserType == AppUser.internalUserType;
            return c => c.UserType == AppUser.internalUserType && c.TenantId == tenantId;
        }
    }
}
