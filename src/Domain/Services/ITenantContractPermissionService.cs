using System.Collections.Generic;

namespace Domain.Service
{
    /// <summary>
    /// Permisos permitidos según módulos contratados del tenant.
    /// Usado al crear roles y al listar permisos (evita asignar packs no contratados).
    /// La edición de un rol por un platform admin no aplica este filtro.
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
