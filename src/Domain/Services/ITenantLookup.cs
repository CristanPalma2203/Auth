namespace Domain.Service
{
    /// <summary>Resuelve TenantId desde codigo de tienda (tempora, carbonera-cacao, …).</summary>
    public interface ITenantLookup
    {
        int? ResolveIdByCode(string code);
    }
}
