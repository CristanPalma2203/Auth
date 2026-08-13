using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Service;
using Domain.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Service
{
    public class TenantContractPermissionService : ITenantContractPermissionService
    {
        private readonly AutenticationContext db;

        public TenantContractPermissionService(AutenticationContext db)
        {
            this.db = db;
        }

        public HashSet<int> AllowedPermissionIds(int? tenantId)
        {
            if (!tenantId.HasValue || tenantId.Value <= 0)
                return null; // sin filtro

            var moduleCodes = db.TenantModules.AsNoTracking()
                .Where(m => m.TenantId == tenantId.Value)
                .Select(m => m.ModuleCode)
                .ToList();

            var wantedCodes = TenantModuleCatalog.PermissionCodesForModules(moduleCodes).ToList();
            var ids = db.Permissions.AsNoTracking()
                .Where(p => wantedCodes.Contains(p.Code))
                .Select(p => p.Id)
                .ToList();

            return new HashSet<int>(ids);
        }
    }
}
