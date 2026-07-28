using Dominio.Models;
using System;

namespace Dominio.Especificaciones
{
    public class BuscarUsuariosPorTenant : ISpecification<Usuario>
    {
        private readonly int? tenantId;
        private readonly bool platformSeesAll;

        public BuscarUsuariosPorTenant(int? actorTenantId, bool isPlatformAdmin)
        {
            tenantId = actorTenantId;
            platformSeesAll = isPlatformAdmin;
        }

        public Func<Usuario, bool> Traer()
        {
            if (platformSeesAll)
                return c => c.UserType == Usuario.usuarioInterno;
            return c => c.UserType == Usuario.usuarioInterno && c.TenantId == tenantId;
        }
    }
}
