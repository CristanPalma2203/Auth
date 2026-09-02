using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Dtos.AppUser;
using Application.Mappers;
using Domain.Specifications;
using Domain.Models;
using Domain.Repositories;
using Domain.Service;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Contracts;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IAppUserRepository appUserRepository;
        private readonly IExternalUserRepository usuarioExternoRepository;
        private readonly ITokenService tokenService;
        private readonly IPermissionRepository permissionRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ITenantLookup tenantLookup;
        private readonly AutenticationContext db;

        public OAuthController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IAppUserRepository appUserRepository,
            IExternalUserRepository usuarioExternoRepository,
            ITokenService tokenService,
            IPermissionRepository permissionRepository,
            IUnitOfWork unitOfWork,
            ITenantLookup tenantLookup,
            AutenticationContext db)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.appUserRepository = appUserRepository;
            this.usuarioExternoRepository = usuarioExternoRepository;
            this.tokenService = tokenService;
            this.permissionRepository = permissionRepository;
            this.unitOfWork = unitOfWork;
            this.tenantLookup = tenantLookup;
            this.db = db;
        }

        [HttpGet("google")]
        public IActionResult GoogleStart([FromQuery] string returnUrl, [FromQuery] string origen)
        {
            var clientId = configuration["OAuth:Google:ClientId"];
            if (string.IsNullOrWhiteSpace(clientId))
                return BadRequest(new { message = "OAuth Google no configurado (OAuth:Google:ClientId)" });

            var redirectUri = GetCallbackUri();
            var state = EncodeOauthState(returnUrl ?? GetDefaultReturnUrl(), origen);

            var url =
                "https://accounts.google.com/o/oauth2/v2/auth" +
                "?response_type=code" +
                "&client_id=" + Uri.EscapeDataString(clientId) +
                "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                "&scope=" + Uri.EscapeDataString("openid email profile") +
                "&state=" + Uri.EscapeDataString(state) +
                "&access_type=online" +
                "&prompt=select_account";

            return Redirect(url);
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state, [FromQuery] string error)
        {
            var oauthState = DecodeOauthState(state);
            var returnUrl = oauthState.ReturnUrl ?? GetDefaultReturnUrl();
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(code))
            {
                return Redirect(AppendQuery(returnUrl, "error=" + Uri.EscapeDataString(error ?? "oauth_denied")));
            }

            try
            {
                var profile = await ExchangeCodeAsync(code);
                var login = UpsertGoogleUser(profile, oauthState.Origen, returnUrl);
                return Redirect(AppendQuery(returnUrl,
                    "token=" + Uri.EscapeDataString(login.Token) +
                    "&tipoUsuario=" + Uri.EscapeDataString(login.UserType ?? "external-user") +
                    "&email=" + Uri.EscapeDataString(login.AccessIdentifier ?? "") +
                    "&nombre=" + Uri.EscapeDataString(login.Name ?? "")));
            }
            catch (Exception ex)
            {
                return Redirect(AppendQuery(returnUrl, "error=" + Uri.EscapeDataString(ex.Message)));
            }
        }

        [HttpPost("google/token")]
        public async Task<ActionResult<UserLoginDto>> GoogleToken([FromBody] GoogleIdTokenRequest body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.IdToken))
                return BadRequest(new { message = "idToken requerido" });

            var profile = await VerifyIdTokenAsync(body.IdToken);
            var login = UpsertGoogleUser(profile, body.Origen, null);
            return Ok(login);
        }

        private async Task<GoogleProfile> ExchangeCodeAsync(string code)
        {
            var clientId = configuration["OAuth:Google:ClientId"];
            var clientSecret = configuration["OAuth:Google:ClientSecret"];
            var redirectUri = GetCallbackUri();
            var client = httpClientFactory.CreateClient();

            var form = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret ?? "",
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code"
            };

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                throw new InvalidOperationException(
                    "OAuth Google incompleto: falta ClientId o ClientSecret en Auth (appsettings.Local.json).");

            var tokenResponse = await client.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(form));
            if (!tokenResponse.IsSuccessStatusCode)
            {
                var raw = await tokenResponse.Content.ReadAsStringAsync();
                var googleError = TryReadGoogleOAuthError(raw);
                throw new InvalidOperationException(
                    string.IsNullOrEmpty(googleError)
                        ? "No se pudo intercambiar el codigo de Google"
                        : "No se pudo intercambiar el codigo de Google: " + googleError);
            }

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            using var tokenDoc = JsonDocument.Parse(tokenJson);
            var idToken = tokenDoc.RootElement.GetProperty("id_token").GetString();
            return await VerifyIdTokenAsync(idToken);
        }

        private async Task<GoogleProfile> VerifyIdTokenAsync(string idToken)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync(
                "https://oauth2.googleapis.com/tokeninfo?id_token=" + Uri.EscapeDataString(idToken));
            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("id_token de Google invalido");

            using var doc = JsonDocument.Parse(json);
            var email = doc.RootElement.GetProperty("email").GetString();
            var emailVerified = doc.RootElement.TryGetProperty("email_verified", out var ev)
                && (ev.ValueKind == JsonValueKind.True
                    || string.Equals(ev.GetString(), "true", StringComparison.OrdinalIgnoreCase));
            var name = doc.RootElement.TryGetProperty("name", out var n) ? n.GetString() : email;

            var expectedAudience = configuration["OAuth:Google:ClientId"];
            if (!string.IsNullOrEmpty(expectedAudience)
                && doc.RootElement.TryGetProperty("aud", out var aud)
                && aud.GetString() != expectedAudience)
            {
                throw new InvalidOperationException("Audience de Google no coincide");
            }

            if (string.IsNullOrWhiteSpace(email) || !emailVerified)
                throw new InvalidOperationException("El correo de Google no esta verificado");

            return new GoogleProfile { Email = email.Trim(), Name = name?.Trim() };
        }

        private UserLoginDto UpsertGoogleUser(GoogleProfile profile, string origen, string returnUrl)
        {
            var tenantId = tenantLookup.ResolveIdByCode(InferOrigen(origen, returnUrl));
            var email = profile.Email.Trim();
            var existente = db.Users.FirstOrDefault(u => u.AccessIdentifier == email);

            if (existente == null)
            {
                var parts = (profile.Name ?? email).Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                var usuarioNuevo = new AppUser
                {
                    AccessIdentifier = email,
                    Name = profile.Name ?? email,
                    Password = Guid.NewGuid().ToString("N") + "Aa1!",
                    TenantId = tenantId
                };
                usuarioNuevo.InitializeExternal(new List<int>());
                usuarioNuevo.Enable();
                db.Users.Add(usuarioNuevo);

                var perfil = new ExternalUser
                {
                    TenantId = tenantId,
                    Name = parts.Length > 0 ? parts[0] : profile.Name,
                    LastName = parts.Length > 1 ? parts[1] : "",
                    Email = email,
                    Identifier = email,
                    Phone = "",
                    Mobile = "",
                    EntryType = "GOOGLE"
                };
                perfil.RegisterAccount();
                perfil.EntryType = "GOOGLE";
                perfil.VerifyEmail();
                db.ExternalUsers.Add(perfil);
                db.SaveChanges();
            }
            else
            {
                if (!existente.IsActive)
                    existente.Enable();
                if (!existente.TenantId.HasValue && tenantId.HasValue)
                    existente.TenantId = tenantId;

                var perfil = db.ExternalUsers.FirstOrDefault(c =>
                    c.Email == email || c.Identifier == email);
                if (perfil == null)
                {
                    var parts = (profile.Name ?? email).Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    perfil = new ExternalUser
                    {
                        TenantId = tenantId ?? existente.TenantId,
                        Name = parts.Length > 0 ? parts[0] : profile.Name,
                        LastName = parts.Length > 1 ? parts[1] : "",
                        Email = email,
                        Identifier = email,
                        Phone = "",
                        Mobile = "",
                        EntryType = "GOOGLE"
                    };
                    perfil.RegisterAccount();
                    perfil.EntryType = "GOOGLE";
                    perfil.VerifyEmail();
                    db.ExternalUsers.Add(perfil);
                }
                else
                {
                    if (!perfil.TenantId.HasValue && tenantId.HasValue)
                        perfil.TenantId = tenantId;

                    if (string.Equals(perfil.EntryType, "deleted", StringComparison.OrdinalIgnoreCase))
                    {
                        perfil.EntryType = "GOOGLE";
                        perfil.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (!perfil.TenantId.HasValue
                        && (string.IsNullOrWhiteSpace(perfil.EntryType) || perfil.EntryType == "WEB"))
                    {
                        perfil.EntryType = "GOOGLE";
                    }

                    perfil.VerifyEmail();
                }

                db.SaveChanges();
            }

            var appUser = appUserRepository.GetUserWithRolePermissions(new FindUserByIdentifier(email));
            var respuesta = UserMappingHelper.ToDtoLogin(appUser, permissionRepository);
            respuesta.Token = tokenService.CreateOrGetToken(appUser);
            return respuesta;
        }

        private static string InferOrigen(string origen, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(origen))
                return origen.Trim().ToLowerInvariant();
            var url = (returnUrl ?? "").ToLowerInvariant();
            if (url.Contains("carbonera") || url.Contains(":5174") || url.Contains(":5175"))
                return "carbonera-cacao";
            return "tempora";
        }

        private static string EncodeOauthState(string returnUrl, string origen)
        {
            var json = JsonSerializer.Serialize(new OauthStateDto
            {
                ReturnUrl = returnUrl,
                Origen = origen
            });
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        }

        private OauthStateDto DecodeOauthState(string state)
        {
            var raw = DecodeState(state);
            if (string.IsNullOrWhiteSpace(raw))
                return new OauthStateDto { ReturnUrl = GetDefaultReturnUrl() };
            if (raw.StartsWith("{"))
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<OauthStateDto>(raw,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (parsed != null && !string.IsNullOrWhiteSpace(parsed.ReturnUrl))
                        return parsed;
                }
                catch
                {
                    // state viejo: URL plana
                }
            }
            return new OauthStateDto { ReturnUrl = raw };
        }

        private string GetCallbackUri()
        {
            var configured = configuration["OAuth:Google:CallbackUrl"];
            if (!string.IsNullOrWhiteSpace(configured)) return configured.Trim();
            return $"{Request.Scheme}://{Request.Host}/api/OAuth/google/callback";
        }

        private string GetDefaultReturnUrl()
        {
            return configuration["OAuth:Google:DefaultReturnUrl"]
                   ?? "http://localhost:3001/auth/callback";
        }

        private static string TryReadGoogleOAuthError(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            try
            {
                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;
                var error = root.TryGetProperty("error", out var e) ? e.GetString() : null;
                var description = root.TryGetProperty("error_description", out var d) ? d.GetString() : null;
                if (string.Equals(error, "invalid_client", StringComparison.OrdinalIgnoreCase))
                    return "invalid_client (ClientSecret no coincide con Google Cloud)";
                if (string.Equals(error, "redirect_uri_mismatch", StringComparison.OrdinalIgnoreCase))
                    return "redirect_uri_mismatch (en Google Cloud debe estar exacto: "
                           + "http://localhost:8081/api/OAuth/google/callback)";
                if (string.Equals(error, "invalid_grant", StringComparison.OrdinalIgnoreCase))
                    return "invalid_grant (codigo usado o expirado; vuelve a Iniciar sesion)";
                if (!string.IsNullOrWhiteSpace(description))
                    return (error ?? "error") + " — " + description;
                return error;
            }
            catch
            {
                return null;
            }
        }

        private static string DecodeState(string state)
        {
            if (string.IsNullOrWhiteSpace(state)) return null;
            try
            {
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(state));
            }
            catch
            {
                return state;
            }
        }

        private static string AppendQuery(string url, string query)
        {
            if (string.IsNullOrEmpty(url)) return "?" + query;
            return url.Contains("?") ? url + "&" + query : url + "?" + query;
        }

        private class OauthStateDto
        {
            public string ReturnUrl { get; set; }
            public string Origen { get; set; }
        }

        private class GoogleProfile
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }
    }

}
