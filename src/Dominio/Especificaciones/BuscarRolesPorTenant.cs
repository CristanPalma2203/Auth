using Dominio.Models;
using System;

namespace Dominio.Especificaciones
{
    public class BuscarRolesPorTenant : ISpecification<Rol>
    {
        private readonly int? tenantId;
        private readonly bool platformSeesAll;
        private readonly bool soloAsignables;

        public BuscarRolesPorTenant(int? actorTenantId, bool isPlatformAdmin, bool soloAsignables = true)
        {
            tenantId = actorTenantId;
            platformSeesAll = isPlatformAdmin;
            this.soloAsignables = soloAsignables;
        }

        public Func<Rol, bool> Traer()
        {
            if (platformSeesAll)
            {
                if (soloAsignables)
                    return c => c.Asignable;
                return c => true;
            }
            if (soloAsignables)
                return c => c.Asignable && c.TenantId == tenantId;
            return c => c.TenantId == tenantId;
        }
    }
}
