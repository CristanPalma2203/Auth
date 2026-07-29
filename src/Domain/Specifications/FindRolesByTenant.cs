using Domain.Models;
using System;

namespace Domain.Specifications
{
    public class FindRolesByTenant : ISpecification<Role>
    {
        private readonly int? tenantId;
        private readonly bool platformSeesAll;
        private readonly bool soloAsignables;

        public FindRolesByTenant(int? actorTenantId, bool isPlatformAdmin, bool soloAsignables = true)
        {
            tenantId = actorTenantId;
            platformSeesAll = isPlatformAdmin;
            this.soloAsignables = soloAsignables;
        }

        public Func<Role, bool> Traer()
        {
            if (platformSeesAll)
            {
                if (soloAsignables)
                    return c => c.IsAssignable;
                return c => true;
            }
            if (soloAsignables)
                return c => c.IsAssignable && c.TenantId == tenantId;
            return c => c.TenantId == tenantId;
        }
    }
}
