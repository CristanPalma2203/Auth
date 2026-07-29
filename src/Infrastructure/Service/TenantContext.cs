using Application.Exceptions;
using Domain.Service;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Infrastructure.Service
{
    public class TenantContext : ITenantContext
    {
        public const string ClaimTenantId = "tenant_id";
        public const string ClaimTenantCodigo = "tenant_codigo";

        private readonly IHttpContextAccessor httpContextAccessor;
        private int? _tenantId;
        private string _tenantCodigo;
        private bool _loaded;

        public TenantContext(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public int? TenantId
        {
            get { EnsureLoaded(); return _tenantId; }
        }

        public string TenantCodigo
        {
            get { EnsureLoaded(); return _tenantCodigo; }
        }

        public bool IsPlatformAdmin => !TenantId.HasValue;

        public void EnsureCanAccessTenant(int? resourceTenantId)
        {
            EnsureSameTenantOrPlatform(resourceTenantId);
        }

        public void EnsureSameTenantOrPlatform(int? resourceTenantId)
        {
            if (IsPlatformAdmin) return;
            if (!resourceTenantId.HasValue || resourceTenantId.Value != TenantId.Value)
                throw new HttpException(403, "No autorizado para este Tenants");
        }

        private void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;
            try
            {
                var auth = httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(auth) || !auth.StartsWith("Bearer "))
                    return;
                var raw = auth.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(raw)) return;
                var token = handler.ReadJwtToken(raw);
                var tid = token.Claims.FirstOrDefault(c => c.Type == ClaimTenantId)?.Value;
                if (int.TryParse(tid, out var id) && id > 0)
                    _tenantId = id;
                _tenantCodigo = token.Claims.FirstOrDefault(c => c.Type == ClaimTenantCodigo)?.Value;
            }
            catch
            {
            }
        }
    }
}
