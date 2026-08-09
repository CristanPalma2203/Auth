using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Infrastructure.Service.Email
{
    public sealed class BrandedEmailRenderer
    {
        private readonly string _layout;

        public BrandedEmailRenderer()
        {
            _layout = ReadResource("email-layout.html");
        }

        public string Render(
            TenantEmailBrand brand,
            string kicker,
            string heading,
            string bodyHtml,
            string ctaUrl = null,
            string ctaLabel = null,
            string footerNote = null)
        {
            brand ??= TenantEmailBrand.PlatformFallback();

            var logoCell = string.IsNullOrWhiteSpace(brand.BrandLogoUrl)
                ? ""
                : $"<td width=\"36\" height=\"36\" valign=\"middle\" style=\"width:36px;height:36px;\">" +
                  $"<img src=\"{WebUtility.HtmlEncode(brand.BrandLogoUrl)}\" width=\"36\" height=\"36\" alt=\"\" " +
                  "style=\"display:block;width:36px;height:36px;border:0;object-fit:contain;\" /></td>";

            var ctaBlock = "";
            if (!string.IsNullOrWhiteSpace(ctaUrl) && !string.IsNullOrWhiteSpace(ctaLabel))
            {
                var safeUrl = WebUtility.HtmlEncode(ctaUrl);
                var safeLabel = WebUtility.HtmlEncode(ctaLabel);
                ctaBlock =
                    "<tr><td align=\"left\" style=\"padding:28px 8px 8px 8px;\">" +
                    "<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-collapse:collapse;\">" +
                    "<tr><td align=\"center\" bgcolor=\"" + brand.BrandPrimaryColor + "\" style=\"background-color:" + brand.BrandPrimaryColor + ";\">" +
                    "<a href=\"" + safeUrl + "\" target=\"_blank\" " +
                    "style=\"display:inline-block;padding:14px 28px;font-family:Arial,Helvetica,sans-serif;font-size:12px;" +
                    "font-weight:600;letter-spacing:0.14em;text-transform:uppercase;line-height:1.4;color:" + brand.CtaInk + ";" +
                    "text-decoration:none;\">" + safeLabel + "</a>" +
                    "</td></tr></table></td></tr>";
            }

            return _layout
                .Replace("{{BrandName}}", WebUtility.HtmlEncode(brand.BrandName ?? ""))
                .Replace("{{BrandPrimaryColor}}", brand.BrandPrimaryColor ?? "#0a0a0a")
                .Replace("{{BrandBgColor}}", brand.BrandBgColor ?? "#f5f5f5")
                .Replace("{{BrandInkColor}}", brand.BrandInkColor ?? "#171717")
                .Replace("{{MutedInk}}", brand.MutedInk ?? "#737373")
                .Replace("{{RuleColor}}", brand.RuleColor ?? "#e5e5e5")
                .Replace("{{LogoCell}}", logoCell)
                .Replace("{{Kicker}}", WebUtility.HtmlEncode(kicker ?? ""))
                .Replace("{{Heading}}", WebUtility.HtmlEncode(heading ?? ""))
                .Replace("{{BodyHtml}}", bodyHtml ?? "")
                .Replace("{{CtaBlock}}", ctaBlock)
                .Replace("{{FooterNote}}", WebUtility.HtmlEncode(footerNote ?? "Si no solicitaste este mensaje, puedes ignorarlo."));
        }

        public static string BuildVerifyBody()
        {
            return "<p style=\"margin:0;\">Confirma tu correo para activar tu cuenta en la tienda. Solo te tomará un momento.</p>";
        }

        private static string ReadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
