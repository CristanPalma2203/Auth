using System;
using Domain.Models;

namespace Infrastructure.Service.Email
{
    /// <summary>Tokens de marca para la plantilla maestra de correo.</summary>
    public sealed class TenantEmailBrand
    {
        public string BrandName { get; init; }
        public string BrandPrimaryColor { get; init; }
        public string BrandBgColor { get; init; }
        public string BrandInkColor { get; init; }
        public string BrandLogoUrl { get; init; }
        public string StorefrontPublicUrl { get; init; }
        public string EmailFromDisplay { get; init; }
        public string MutedInk { get; init; }
        public string RuleColor { get; init; }
        public string CtaInk { get; init; }

        public static TenantEmailBrand PlatformFallback()
        {
            return new TenantEmailBrand
            {
                BrandName = "ERP Base",
                BrandPrimaryColor = "#0a0a0a",
                BrandBgColor = "#f5f5f5",
                BrandInkColor = "#171717",
                BrandLogoUrl = null,
                StorefrontPublicUrl = null,
                EmailFromDisplay = "ERP Base",
                MutedInk = "#737373",
                RuleColor = "#e5e5e5",
                CtaInk = "#ffffff"
            };
        }

        public static TenantEmailBrand FromTenant(Tenant tenant)
        {
            if (tenant == null) return PlatformFallback();

            var bg = Or(tenant.BrandBgColor, "#f5f5f5");
            var ink = Or(tenant.BrandInkColor, "#171717");
            var primary = Or(tenant.BrandPrimaryColor, "#0a0a0a");
            var darkBg = IsDark(bg);

            return new TenantEmailBrand
            {
                BrandName = Or(tenant.BrandName, tenant.Name, "ERP Base"),
                BrandPrimaryColor = primary,
                BrandBgColor = bg,
                BrandInkColor = ink,
                BrandLogoUrl = TrimOrNull(tenant.BrandLogoUrl),
                StorefrontPublicUrl = TrimOrNull(tenant.StorefrontPublicUrl)?.TrimEnd('/'),
                EmailFromDisplay = Or(tenant.EmailFromDisplay, tenant.BrandName, tenant.Name, "ERP Base"),
                MutedInk = darkBg ? "rgba(242,237,228,0.62)" : "#737373",
                RuleColor = darkBg ? "rgba(242,237,228,0.18)" : "#e5e5e5",
                CtaInk = IsDark(primary) ? "#ffffff" : "#031424"
            };
        }

        private static string Or(params string[] values)
        {
            foreach (var v in values)
            {
                if (!string.IsNullOrWhiteSpace(v)) return v.Trim();
            }
            return null;
        }

        private static string TrimOrNull(string value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static bool IsDark(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return false;
            hex = hex.Trim().TrimStart('#');
            if (hex.Length == 3)
                hex = string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);
            if (hex.Length != 6) return false;
            if (!int.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out var r)) return false;
            if (!int.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out var g)) return false;
            if (!int.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out var b)) return false;
            var luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255.0;
            return luminance < 0.45;
        }
    }
}
