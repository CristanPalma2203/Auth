using System;
using System.Collections.Generic;
using System.Linq;
using Application.Exceptions;
using Application.Services.Validaciones;
using Domain.Helpers;
using Domain.Models;
using Domain.Service;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    /// <summary>
    /// Administración de empresas (tenants). Solo el admin de plataforma crea empresas;
    /// el admin de un tenant únicamente puede consultar y editar la suya.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly AutenticationContext db;
        private readonly IAutenticationHelper autenticationHelper;
        private readonly ITenantContext tenantContext;
        private readonly IEmailHelper emailHelper;

        public TenantController(
            AutenticationContext db,
            IAutenticationHelper autenticationHelper,
            ITenantContext tenantContext,
            IEmailHelper emailHelper)
        {
            this.db = db;
            this.autenticationHelper = autenticationHelper;
            this.tenantContext = tenantContext;
            this.emailHelper = emailHelper;
        }

        public class TenantBody
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Nit { get; set; }
            public string Nrc { get; set; }
            public string RazonSocial { get; set; }
            public string Phone { get; set; }
            public string Website { get; set; }
            public int? BusinessTypeId { get; set; }
            /// <summary>Texto opcional que acompaña al cobro en la página pública.</summary>
            public string CheckoutMessage { get; set; }
            public string BrandName { get; set; }
            public string BrandPrimaryColor { get; set; }
            public string BrandBgColor { get; set; }
            public string BrandInkColor { get; set; }
            public string BrandLogoUrl { get; set; }
            public string StorefrontPublicUrl { get; set; }
            public string EmailFromDisplay { get; set; }
            public bool? IsActive { get; set; }
        }

        [HttpGet]
        public object Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string name = null)
        {
            Authorize("tenant-list", "tenants");

            var q = db.Tenants.AsNoTracking().AsQueryable();
            if (!tenantContext.IsPlatformAdmin)
                q = q.Where(t => t.Id == tenantContext.TenantId);
            if (!string.IsNullOrWhiteSpace(name))
                q = q.Where(t => t.Name.Contains(name) || t.Code.Contains(name));

            var total = q.Count();
            var values = q
                .OrderBy(t => t.Code)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new
                {
                    t.Id,
                    t.Code,
                    t.Name,
                    t.Nit,
                    t.Nrc,
                    t.RazonSocial,
                    t.Phone,
                    t.Website,
                    t.BusinessTypeId,
                    t.IsActive,
                    t.CreatedAt
                })
                .ToList();

            return new
            {
                values,
                metadata = new
                {
                    totalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize),
                    totalCount = total,
                    pageNumber,
                    pageSize
                }
            };
        }

        [HttpGet("unpaged")]
        public object GetUnpaged()
        {
            Authorize("tenant-list", "tenants");

            var q = db.Tenants.AsNoTracking().Where(t => t.IsActive);
            if (!tenantContext.IsPlatformAdmin)
                q = q.Where(t => t.Id == tenantContext.TenantId);

            return new { values = q.OrderBy(t => t.Name).Select(t => new { t.Id, t.Code, t.Name }).ToList() };
        }

        [HttpGet("{id:int}")]
        public object GetById(int id)
        {
            Authorize("tenant-view", "tenant-edit", "tenants");
            tenantContext.EnsureSameTenantOrPlatform(id);

            var tenant = db.Tenants.AsNoTracking().FirstOrDefault(t => t.Id == id)
                ?? throw new HttpException(404, "Empresa no encontrada");

            return MapTenant(tenant);
        }

        /// <summary>HTML de vista previa del correo de verificación (marca del tenant).</summary>
        [HttpGet("{id:int}/email-preview")]
        public IActionResult EmailPreview(int id, [FromQuery] string kind = "verify")
        {
            Authorize("tenant-view", "tenant-edit", "tenants");
            tenantContext.EnsureSameTenantOrPlatform(id);

            if (!db.Tenants.AsNoTracking().Any(t => t.Id == id))
                throw new HttpException(404, "Empresa no encontrada");

            var k = (kind ?? "verify").Trim().ToLowerInvariant();
            if (k != "verify")
                throw new HttpException(422, "kind soportado en Auth: verify");

            var html = emailHelper.RenderVerificationPreview(id);
            return Content(html, "text/html; charset=utf-8");
        }

        [HttpPost]
        public object Post([FromBody] TenantBody body)
        {
            Authorize("tenant-create");
            if (!tenantContext.IsPlatformAdmin)
                throw new HttpException(403, "Solo el administrador de plataforma puede crear empresas");

            if (body == null) throw new HttpException(422, "Body requerido");
            var code = Normalize(body.Code);
            if (string.IsNullOrWhiteSpace(code)) throw new HttpException(422, "El código es obligatorio");
            if (string.IsNullOrWhiteSpace(body.Name)) throw new HttpException(422, "El nombre es obligatorio");
            if (db.Tenants.Any(t => t.Code == code))
                throw new HttpException(422, "Ya existe una empresa con ese código");

            var tenant = new Tenant
            {
                Code = code,
                Name = body.Name.Trim(),
                Nit = Trim(body.Nit),
                Nrc = Trim(body.Nrc),
                RazonSocial = Trim(body.RazonSocial),
                Phone = Trim(body.Phone),
                Website = Trim(body.Website),
                BusinessTypeId = body.BusinessTypeId,
                CheckoutMessage = Trim(body.CheckoutMessage),
                BrandName = Trim(body.BrandName),
                BrandPrimaryColor = Trim(body.BrandPrimaryColor),
                BrandBgColor = Trim(body.BrandBgColor),
                BrandInkColor = Trim(body.BrandInkColor),
                BrandLogoUrl = Trim(body.BrandLogoUrl),
                StorefrontPublicUrl = TrimUrl(body.StorefrontPublicUrl),
                EmailFromDisplay = Trim(body.EmailFromDisplay),
                IsActive = body.IsActive ?? true,
                CreatedAt = DateTime.UtcNow
            };

            db.Tenants.Add(tenant);
            db.SaveChanges();

            return MapTenant(tenant);
        }

        [HttpPut("{id:int}")]
        public object Put(int id, [FromBody] TenantBody body)
        {
            Authorize("tenant-edit");
            tenantContext.EnsureSameTenantOrPlatform(id);

            if (body == null) throw new HttpException(422, "Body requerido");
            var tenant = db.Tenants.FirstOrDefault(t => t.Id == id)
                ?? throw new HttpException(404, "Empresa no encontrada");

            if (!string.IsNullOrWhiteSpace(body.Name)) tenant.Name = body.Name.Trim();
            if (body.Nit != null) tenant.Nit = Trim(body.Nit);
            if (body.Nrc != null) tenant.Nrc = Trim(body.Nrc);
            if (body.RazonSocial != null) tenant.RazonSocial = Trim(body.RazonSocial);
            if (body.Phone != null) tenant.Phone = Trim(body.Phone);
            if (body.Website != null) tenant.Website = Trim(body.Website);
            if (body.BusinessTypeId.HasValue) tenant.BusinessTypeId = body.BusinessTypeId;
            if (body.CheckoutMessage != null) tenant.CheckoutMessage = Trim(body.CheckoutMessage);
            if (body.BrandName != null) tenant.BrandName = Trim(body.BrandName);
            if (body.BrandPrimaryColor != null) tenant.BrandPrimaryColor = Trim(body.BrandPrimaryColor);
            if (body.BrandBgColor != null) tenant.BrandBgColor = Trim(body.BrandBgColor);
            if (body.BrandInkColor != null) tenant.BrandInkColor = Trim(body.BrandInkColor);
            if (body.BrandLogoUrl != null) tenant.BrandLogoUrl = Trim(body.BrandLogoUrl);
            if (body.StorefrontPublicUrl != null) tenant.StorefrontPublicUrl = TrimUrl(body.StorefrontPublicUrl);
            if (body.EmailFromDisplay != null) tenant.EmailFromDisplay = Trim(body.EmailFromDisplay);

            // El código identifica al tenant en storefronts/config: solo plataforma puede cambiarlo.
            if (tenantContext.IsPlatformAdmin)
            {
                var code = Normalize(body.Code);
                if (!string.IsNullOrWhiteSpace(code) && code != tenant.Code)
                {
                    if (db.Tenants.Any(t => t.Code == code && t.Id != id))
                        throw new HttpException(422, "Ya existe una empresa con ese código");
                    tenant.Code = code;
                }
                if (body.IsActive.HasValue) tenant.IsActive = body.IsActive.Value;
            }

            db.SaveChanges();
            return MapTenant(tenant);
        }

        private static object MapTenant(Tenant tenant) => new
        {
            tenant.Id,
            tenant.Code,
            tenant.Name,
            tenant.Nit,
            tenant.Nrc,
            tenant.RazonSocial,
            tenant.Phone,
            tenant.Website,
            tenant.BusinessTypeId,
            tenant.CheckoutMessage,
            tenant.BrandName,
            tenant.BrandPrimaryColor,
            tenant.BrandBgColor,
            tenant.BrandInkColor,
            tenant.BrandLogoUrl,
            tenant.StorefrontPublicUrl,
            tenant.EmailFromDisplay,
            tenant.IsActive,
            tenant.CreatedAt
        };

        private void Authorize(params string[] permissions) =>
            autenticationHelper.Autenticado(new List<string>(permissions));

        private static string Trim(string value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static string TrimUrl(string value)
        {
            var t = Trim(value);
            return t?.TrimEnd('/');
        }

        private static string Normalize(string code) =>
            string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToLowerInvariant();
    }
}
