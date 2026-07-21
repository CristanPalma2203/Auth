using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Aplicacion.Dtos.Usuario;
using Aplicacion.Mappers;
using Dominio.Especificaciones;
using Dominio.Models;
using Dominio.Repositories;
using Dominio.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IUsuarioExternoRepository usuarioExternoRepository;
        private readonly ITokenService tokenService;
        private readonly IPermisoRepository permisoRepository;
        private readonly IUnitOfWork unitOfWork;

        public OAuthController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IUsuarioRepository usuarioRepository,
            IUsuarioExternoRepository usuarioExternoRepository,
            ITokenService tokenService,
            IPermisoRepository permisoRepository,
            IUnitOfWork unitOfWork)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.usuarioRepository = usuarioRepository;
            this.usuarioExternoRepository = usuarioExternoRepository;
            this.tokenService = tokenService;
            this.permisoRepository = permisoRepository;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("google")]
        public IActionResult GoogleStart([FromQuery] string returnUrl)
        {
            var clientId = configuration["OAuth:Google:ClientId"];
            if (string.IsNullOrWhiteSpace(clientId))
                return BadRequest(new { message = "OAuth Google no configurado (OAuth:Google:ClientId)" });

            var redirectUri = GetCallbackUri();
            var state = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(returnUrl ?? GetDefaultReturnUrl()));

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
            var returnUrl = DecodeState(state) ?? GetDefaultReturnUrl();
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(code))
            {
                return Redirect(AppendQuery(returnUrl, "error=" + Uri.EscapeDataString(error ?? "oauth_denied")));
            }

            try
            {
                var profile = await ExchangeCodeAsync(code);
                var login = UpsertGoogleUser(profile);
                return Redirect(AppendQuery(returnUrl,
                    "token=" + Uri.EscapeDataString(login.Token) +
                    "&tipoUsuario=" + Uri.EscapeDataString(login.TipoUsuario ?? "usuario-externo")));
            }
            catch (Exception ex)
            {
                return Redirect(AppendQuery(returnUrl, "error=" + Uri.EscapeDataString(ex.Message)));
            }
        }

        [HttpPost("google/token")]
        public async Task<ActionResult<DtoUsuarioLogin>> GoogleToken([FromBody] GoogleIdTokenRequest body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.IdToken))
                return BadRequest(new { message = "idToken requerido" });

            var profile = await VerifyIdTokenAsync(body.IdToken);
            var login = UpsertGoogleUser(profile);
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

            var tokenResponse = await client.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(form));
            if (!tokenResponse.IsSuccessStatusCode)
                throw new InvalidOperationException("No se pudo intercambiar el codigo de Google");

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

        private DtoUsuarioLogin UpsertGoogleUser(GoogleProfile profile)
        {
            var existente = usuarioRepository.Filter(new BuscarUsuarioPorIdentificador(profile.Email)).FirstOrDefault();

            if (existente == null)
            {
                var parts = (profile.Name ?? profile.Email).Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                var usuarioNuevo = new Usuario
                {
                    IdentificadorAcceso = profile.Email,
                    Nombre = profile.Name ?? profile.Email,
                    Contrasena = Guid.NewGuid().ToString("N") + "Aa1!"
                };
                usuarioNuevo.InicializarExterno(new List<int>());
                usuarioNuevo.Enable();
                usuarioRepository.Create(usuarioNuevo);

                var perfil = new UsuarioExterno
                {
                    Nombre = parts.Length > 0 ? parts[0] : profile.Name,
                    Apellidos = parts.Length > 1 ? parts[1] : "",
                    Correo = profile.Email,
                    Identificador = profile.Email,
                    Telefono = "",
                    Celular = ""
                };
                perfil.RegistrarCuenta();
                perfil.VerificarCorreo();
                usuarioExternoRepository.Create(perfil);
                unitOfWork.Save();
            }
            else if (!existente.Activo)
            {
                var user = usuarioRepository.GetUsuarioConRolPermiso(new BuscarUsuarioPorIdentificador(profile.Email));
                user.Enable();
                unitOfWork.Save();
            }

            var usuario = usuarioRepository.GetUsuarioConRolPermiso(new BuscarUsuarioPorIdentificador(profile.Email));
            var respuesta = UsuarioMappingHelper.ToDtoLogin(usuario, permisoRepository);
            respuesta.Token = tokenService.CrearOtraerToken(usuario);
            return respuesta;
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

        private class GoogleProfile
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }
    }

    public class GoogleIdTokenRequest
    {
        public string IdToken { get; set; }
    }
}
