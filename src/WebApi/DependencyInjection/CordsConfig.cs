using System;
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

        public static readonly string[] DefaultRemoteOrigins =
        {
            "https://corelux-erp-stg.pages.dev",
            "https://corelux-erp.pages.dev",
        };

        public static void AddCorsConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {
                var configured = configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>()?
                    .Where(origin => !string.IsNullOrWhiteSpace(origin))
                    ?? Array.Empty<string>();

                var allowedOrigins = configured
                    .Concat(DefaultLocalOrigins)
                    .Concat(DefaultRemoteOrigins)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                builder
                    .SetIsOriginAllowed(origin =>
                        !string.IsNullOrWhiteSpace(origin)
                        && allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
        }
    }
}
