using System.Linq;
using Domain.Service;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Service
{
    public class TenantLookup : ITenantLookup
    {
        private readonly AutenticationContext db;

        public TenantLookup(AutenticationContext db)
        {
            this.db = db;
        }

        public int? ResolveIdByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            var c = code.Trim().ToLowerInvariant();
            // Alias legacy / cortos → Code en dbo.tenant
            if (c == "storefront") c = "tempora";
            if (c == "carbonera" || c == "finca" || c == "finca-carbonera")
                c = "carbonera-cacao";

            return db.Tenants.AsNoTracking()
                .Where(t => t.IsActive && t.Code == c)
                .Select(t => (int?)t.Id)
                .FirstOrDefault();
        }
    }
}
