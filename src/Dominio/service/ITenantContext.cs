namespace Dominio.Service
{
    /// <summary>
    /// Contexto de empresa del usuario autenticado.
    /// TenantId null = platform admin (sin aislamiento).
    /// </summary>
    public interface ITenantContext
    {
        int? TenantId { get; }
        string TenantCodigo { get; }
        bool IsPlatformAdmin { get; }
        void EnsureCanAccessTenant(int? resourceTenantId);
        void EnsureSameTenantOrPlatform(int? resourceTenantId);
    }
}
