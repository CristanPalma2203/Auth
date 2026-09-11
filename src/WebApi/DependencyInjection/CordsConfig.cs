using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.DependencyInjection
{
    public static class CordsConfig
    {
        /// <summary>
        /// Orígenes locales alineados con ERP (:3000) y stores (Tempora :5173, Finca :5174/:5175).
        /// </summary>
        public static readonly string[] DefaultLocalOrigins =
        {
            "http://localhost:3000",
            "http://localhost:3001",
            "http://localhost:5173",
            "http://localhost:5174",
            "http://localhost:5175",
            "http://127.0.0.1:3000",
            "http://127.0.0.1:3001",
            "http://127.0.0.1:5173",
            "http://127.0.0.1:5174",
            "http://127.0.0.1:5175",
        };

        /// <summary>
        /// Pages + custom storefront hosts. Always merged after
        /// <c>Cors:AllowedOrigins</c> so an ACA env override that only lists www
        /// cannot drop apex (QA hits https://temporasv.com).
        /// </summary>
        public static readonly string[] DefaultRemoteOrigins =
        {
            "https://corelux-erp-stg.pages.dev",
            "https://corelux-erp.pages.dev",
            "https://corelux-tempora-stg.pages.dev",
            "https://corelux-tempora.pages.dev",
            "https://temporasv.com",
            "https://www.temporasv.com",
        };

        public static void AddCorsConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {
                var allowedOrigins = ResolveAllowedOrigins(configuration);

                builder
                    .SetIsOriginAllowed(origin => IsAllowedOrigin(origin, allowedOrigins))
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
        }

        /// <summary>
        /// File/env <c>Cors:AllowedOrigins</c> plus <see cref="DefaultLocalOrigins"/>
        /// and <see cref="DefaultRemoteOrigins"/>.
        /// Bound as JSON array, indexed env (<c>Cors__AllowedOrigins__0</c>),
        /// or a single <c>Cors__AllowedOrigins</c> string (comma/semicolon).
        /// </summary>
        public static string[] ResolveAllowedOrigins(IConfiguration configuration)
        {
            var configured = ReadConfiguredOrigins(configuration);

            return configured
                .Concat(DefaultLocalOrigins)
                .Concat(DefaultRemoteOrigins)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        /// <summary>
        /// ASP.NET Core maps <c>Cors__AllowedOrigins__N</c> to array indices
        /// (overrides the same JSON index). A scalar <c>Cors__AllowedOrigins</c>
        /// is also accepted so ACA can set one app setting without waiting on image.
        /// </summary>
        internal static string[] ReadConfiguredOrigins(IConfiguration configuration)
        {
            var section = configuration.GetSection("Cors:AllowedOrigins");
            var origins = new List<string>();

            var fromArray = section.Get<string[]>();
            if (fromArray != null)
            {
                origins.AddRange(fromArray.Where(origin => !string.IsNullOrWhiteSpace(origin)));
            }

            var scalar = section.Value;
            if (!string.IsNullOrWhiteSpace(scalar))
            {
                origins.AddRange(scalar
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(origin => origin.Trim())
                    .Where(origin => origin.Length > 0));
            }

            return origins.ToArray();
        }

        /// <summary>
        /// Lista fija + https://corelux-*.pages.dev (alias prod)
        /// y previews/branch https://&lt;hash&gt;.corelux-*.pages.dev
        /// </summary>
        private static bool IsAllowedOrigin(string origin, string[] allowedOrigins)
        {
            if (string.IsNullOrWhiteSpace(origin)) return false;
            if (allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
                return true;
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                return false;
            if (uri.Scheme != Uri.UriSchemeHttps) return false;
            var host = uri.Host;
            if (!host.EndsWith(".pages.dev", StringComparison.OrdinalIgnoreCase))
                return false;
            return host.StartsWith("corelux-", StringComparison.OrdinalIgnoreCase)
                || host.Contains(".corelux-", StringComparison.OrdinalIgnoreCase);
        }
    }
}
