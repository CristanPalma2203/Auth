using System.Collections.Generic;

namespace Domain.Service
{
    /// <summary>
    /// Permisos permitidos según módulos contratados del tenant.
    /// Usado al crear/editar roles (evita asignar packs no contratados).
    /// </summary>
    public interface ITenantContractPermissionService
    {
        /// <summary>
        /// IDs de permission permitidos para un tenant.
        /// null tenantId = sin filtro (roles de plataforma).
        /// </summary>
        HashSet<int> AllowedPermissionIds(int? tenantId);
    }
}
