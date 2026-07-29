using Domain.Models;
using Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Service
{
    public class TokenService : ITokenService
    {
        private readonly AppSettings appSettings;
        private readonly IDistributedCache cache;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TokenService(IOptions<AppSettings> appSettings, IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
        {
            this.appSettings = appSettings.Value;
            this.cache = cache;
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetUserIdentifier()
        {
            var clain = GetToken().Claims.Where(c => c.Type == "email").FirstOrDefault();
            return clain.Value;
        }


        public string CreateOrGetToken(AppUser appUser)
        {
            var tokenCreado = GetExistingToken(appUser.Id);
            if (!string.IsNullOrWhiteSpace(tokenCreado) && TokenMatchesUser(tokenCreado, appUser))
                return tokenCreado;

            // Recrear si no hay token o el cache no trae los claims de Tenants actuales
            cache.Remove(appUser.Id.ToString());

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(BuildClaims(appUser)),
                Expires = DateTime.UtcNow.AddHours(appSettings.TokenTiempoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var obj = new CacheObj { Token = tokenHandler.WriteToken(token), Permissions = this.BuildPermissions(appUser) };

            byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(appSettings.TokenTiempoHoras));

            var id =  appUser.Id.ToString();
            cache.Set(id, encodedCurrentTimeUTC, options);
            return tokenHandler.WriteToken(token);
        }

        private static bool TokenMatchesUser(string token, AppUser appUser)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token)) return false;
                var jwt = handler.ReadJwtToken(token);
                var tidClaim = jwt.Claims.FirstOrDefault(c => c.Type == TenantContext.ClaimTenantId)?.Value;
                var codeClaim = jwt.Claims.FirstOrDefault(c => c.Type == TenantContext.ClaimTenantCodigo)?.Value;

                if (!appUser.TenantId.HasValue)
                    return string.IsNullOrEmpty(tidClaim);

                if (!int.TryParse(tidClaim, out var tid) || tid != appUser.TenantId.Value)
                    return false;

                var expectedCode = appUser.Tenant?.Code;
                if (!string.IsNullOrWhiteSpace(expectedCode)
                    && !string.Equals(codeClaim, expectedCode, StringComparison.OrdinalIgnoreCase))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public List<Permission> GetPermissions()
        {
          
            var cahe = cache.Get(GetIdCache());
            if (cahe != null) return  GetObj(cahe).Permissions;
            return new List<Permission>();
        }

        public bool VerifyToken()
        {
            var objArray = cache.Get(this.GetIdCache());
            if (objArray != null)
                return GetObj(objArray).Token.Equals(GetTokenFromRequest());
            return false;
        }

        private CacheObj GetObj(byte[] cahe) {
            var json = Encoding.Default.GetString(cahe);
            var obj = JsonSerializer.Deserialize<CacheObj>(json);
            return obj;
        }
        private List<Permission> BuildPermissions(AppUser appUser)
        {
            var permisos = new List<Permission>();
            foreach (var roles in appUser.Roles)
            {
                foreach (var Permissions in roles.Role.Permissions)
                {
                    permisos.Add(Permissions.Permission);
                }
            }
            return permisos;
        }


        private JwtSecurityToken GetToken()
        {
         
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(GetTokenFromRequest());
        }


        public string GetTokenFromRequest()
        {
            var tokens= httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            return tokens.Replace("Bearer ", "");

        }

        public int GetUserId()
        {
                var clain = GetToken().Claims.Where(c => c.Type == "nameid").FirstOrDefault();
                return int.Parse(clain.Value);
       
        }

        public int? GetTenantId()
        {
            var claim = GetToken().Claims.FirstOrDefault(c => c.Type == TenantContext.ClaimTenantId);
            if (claim != null && int.TryParse(claim.Value, out var id) && id > 0)
                return id;
            return null;
        }

        public string GetTenantCodigo()
        {
            return GetToken().Claims.FirstOrDefault(c => c.Type == TenantContext.ClaimTenantCodigo)?.Value;
        }

        private static Claim[] BuildClaims(AppUser appUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, appUser.AccessIdentifier.Trim()),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new Claim(ClaimTypes.Surname, ""),
            };
            if (appUser.TenantId.HasValue)
            {
                claims.Add(new Claim(TenantContext.ClaimTenantId, appUser.TenantId.Value.ToString()));
                if (appUser.Tenant != null && !string.IsNullOrWhiteSpace(appUser.Tenant.Code))
                    claims.Add(new Claim(TenantContext.ClaimTenantCodigo, appUser.Tenant.Code));
            }
            return claims.ToArray();
        }

        public void RemoveToken()
        {
            cache.Remove(GetIdCache());
        }
        private string GetIdCache()
        {

            var tokeLeido = GetToken();
            var id = tokeLeido.Claims.Where(c => c.Type == "nameid").FirstOrDefault().Value;
            return id;
        }

        public string GetExistingToken(int userId)
        {
            var id = userId.ToString();
            var objArray = cache.Get(id);
            if (objArray == null) {
                return null;
            }
            return GetObj(objArray).Token;
        }


    }
}

namespace Infrastructure.Service
{
    public class CacheObj
    {
        public string Token { get; set; }
        public List<Permission> Permissions { get; set; }

    }
}