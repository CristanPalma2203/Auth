using Domain.Helpers;
using Domain.Models;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Service.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stubble.Core.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Service
{
    public class EmailHelper : IEmailHelper
    {
        private readonly string rutaHtmlUsiario = "user-created.html";
        private readonly string rutaHtmlRolCreado = "role-created.html";
        private readonly string rutaHtmlRoleEditado = "role-edited.html";
        private readonly string rutaHtmlActulizacionCorreoImportador = "external-user-email-update.html";
        private readonly string rutaHtmlDenegacionAcceso = "access-denied.html";
        private readonly string rutaAccesosImportador = "external-user-access.html";
        private readonly string rutaHtmlSolicitudAcceso = "temporary-code.html";
        private readonly BrandedEmailRenderer brandedRenderer = new BrandedEmailRenderer();
        private readonly ResendEmailClient resendClient;
        private readonly IConfiguration configuration;
        private readonly IRoleRepository roleRepository;
        private readonly AutenticationContext db;

        public EmailHelper(
            ResendEmailClient resendClient,
            IConfiguration configuration,
            IRoleRepository roleRepository,
            AutenticationContext db)
        {
            this.resendClient = resendClient;
            this.configuration = configuration;
            this.roleRepository = roleRepository;
            this.db = db;
        }
        public void SendUserCreatedEmail(string AppUser, string password, string correoDestino)
        {
            var html = ReadResource(rutaHtmlUsiario);
            html = html.Replace("username", AppUser);
            html = html.Replace("password", password);
            html = html.Replace("URLPORTAL", configuration.GetValue<string>("AppSettings:DireccionPortal"));
            SendMsj("Nuevo usuario — ERP Base", correoDestino, html);
        }
        public void SendRoleCreatedEmail(string AppUser, string NombreRol)
        {
            string correoDestino = configuration.GetValue<string>("AppSettings:EmailAdmin");
            var html = ReadResource(rutaHtmlRolCreado);
            html = html.Replace("username", AppUser);
            html = html.Replace("roleName", NombreRol);
            html = html.Replace("URLPORTAL", configuration.GetValue<string>("AppSettings:DireccionPortal"));
            SendMsj("Nuevo rol — ERP Base", correoDestino, html);
        }
        public void SendRoleEditedEmail(string AppUser, string NombreRol)
        {
            string correoDestino = configuration.GetValue<string>("AppSettings:EmailAdmin");
            var html = ReadResource(rutaHtmlRoleEditado);
            html = html.Replace("username", AppUser);
            html = html.Replace("roleName", NombreRol);
            html = html.Replace("URLPORTAL", configuration.GetValue<string>("AppSettings:DireccionPortal"));
            SendMsj("Rol actualizado — ERP Base", correoDestino, html);
        }
        void IEmailHelper.SendVerificationEmail(string correoDestino, string tokenVerificacion)
        {
            ((IEmailHelper)this).SendVerificationEmail(correoDestino, tokenVerificacion, null, null);
        }

        void IEmailHelper.SendVerificationEmail(string correoDestino, string tokenVerificacion, string verificarBaseUrl)
        {
            ((IEmailHelper)this).SendVerificationEmail(correoDestino, tokenVerificacion, verificarBaseUrl, null);
        }

        void IEmailHelper.SendVerificationEmail(
            string correoDestino,
            string tokenVerificacion,
            string verificarBaseUrl,
            int? tenantId)
        {
            var brand = ResolveBrand(tenantId);
            var baseUrl = ResolveVerifyBaseUrl(verificarBaseUrl, brand);
            var url = baseUrl.TrimEnd('/') + "/" + tokenVerificacion;
            var html = brandedRenderer.Render(
                brand,
                kicker: "Crear cuenta",
                heading: "Casi listos",
                bodyHtml: BrandedEmailRenderer.BuildVerifyBody(),
                ctaUrl: url,
                ctaLabel: "Verificar correo",
                footerNote: "Si no creaste esta cuenta, puedes ignorar este mensaje.");
            SendMsj($"Verifica tu correo — {brand.BrandName}", correoDestino, html);
        }

        public string RenderVerificationPreview(int? tenantId)
        {
            var brand = ResolveBrand(tenantId);
            var baseUrl = ResolveVerifyBaseUrl(null, brand);
            var url = baseUrl.TrimEnd('/') + "/preview-token-demo";
            return brandedRenderer.Render(
                brand,
                kicker: "Crear cuenta",
                heading: "Casi listos",
                bodyHtml: BrandedEmailRenderer.BuildVerifyBody()
                    + "<p style=\"margin:16px 0 0 0;font-size:13px;\">Vista previa — el enlace real llega al correo del cliente.</p>",
                ctaUrl: url,
                ctaLabel: "Verificar correo",
                footerNote: "Si no creaste esta cuenta, puedes ignorar este mensaje.");
        }

        private TenantEmailBrand ResolveBrand(int? tenantId)
        {
            if (!tenantId.HasValue || tenantId.Value <= 0)
                return TenantEmailBrand.PlatformFallback();
            var tenant = db.Tenants.AsNoTracking().FirstOrDefault(t => t.Id == tenantId.Value);
            return TenantEmailBrand.FromTenant(tenant);
        }

        private string ResolveVerifyBaseUrl(string verificarBaseUrl, TenantEmailBrand brand)
        {
            if (!string.IsNullOrWhiteSpace(verificarBaseUrl))
                return verificarBaseUrl.TrimEnd('/');

            if (!string.IsNullOrWhiteSpace(brand?.StorefrontPublicUrl))
                return brand.StorefrontPublicUrl.TrimEnd('/') + "/verificar-correo";

            var storefrontVerify = configuration.GetValue<string>("AppSettings:VerifyEmailStorefront");
            if (!string.IsNullOrWhiteSpace(storefrontVerify))
                return storefrontVerify.TrimEnd('/');

            return (configuration.GetValue<string>("AppSettings:VerifyEmail")
                    ?? "http://localhost:3000/#/verificar-correo").TrimEnd('/');
        }
        void IEmailHelper.SendEmailUpdateNotification(string correoDestino, string tokenVerificacion, DateTime fechaActulizacion,string correoNuevo)
        {

            var html = ReadResource(rutaHtmlActulizacionCorreoImportador);
            html = html.Replace("email", correoNuevo);
            html = html.Replace("updatedAt", fechaActulizacion.ToString());
            html = html.Replace("URLPORTAL", configuration.GetValue<string>("AppSettings:DireccionPortal"));
            SendMsj("Email Address Update", correoDestino, html);
        }


        public string ReadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void SendAccessDeniedEmail(string correoDestino, string motivo)
        {
            var html = ReadResource(rutaHtmlDenegacionAcceso);
            html = html.Replace("reason", motivo);
            SendMsj("Acceso denegado — ERP Base", correoDestino, html);
        }

        private void SendMsj(string subject, string correoDestino, string html)
        {
            resendClient.Send(correoDestino, subject, html);
        }
        public void SendExternalUserAccessEmail(ExternalUser externalUser)
        {

            var html = ReadResource(rutaAccesosImportador);
            var stubble = new StubbleBuilder().Build();
            html = stubble.Render(html,  new { ExternalUser = externalUser, Roles = "hoola test", Url = configuration.GetValue<string>("AppSettings:DireccionPortal") });
            SendMsj("Acceso — ERP Base", externalUser.Email, html);
        }
        public void SendRequestUpdateEmail(List<string> correoDestino, string motivo, string TemporaryCode)
        {
            var html = ReadResource(rutaHtmlSolicitudAcceso);
            var stubble = new StubbleBuilder().Build();
            html = html.Replace("temporaryCode", TemporaryCode);
            html = html.Replace("reason", motivo);
            foreach (var item in correoDestino)
            {
                SendMsj("Código de acceso — ERP Base", item, html);
            }

        }
       
    }
}
