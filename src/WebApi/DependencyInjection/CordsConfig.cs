using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.DependencyInjection
{
    public static class CordsConfig
    {
        public static void AddCorsConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {
                var allowedOrigins = configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>()?
                    .Where(origin => !string.IsNullOrWhiteSpace(origin))
                    .ToArray();

                if (allowedOrigins == null || allowedOrigins.Length == 0)
                {
                    allowedOrigins = new[] { "http://localhost:3000", "http://localhost:5173" };
                }

                builder.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
            }));
        }
    }

}
