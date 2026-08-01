using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Infrastructure.Service;

namespace WebApi.DependencyInjection
{
    public static class RedisExtencion
    {
        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings").Get<AppSettings>();
            var redis = appSettingsSection?.ConnectionStringsRedis?.Trim();

            if (string.IsNullOrEmpty(redis))
            {
                services.AddDistributedMemoryCache();
                return;
            }

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redis;
                options.InstanceName = "";
            });
        }
    }
}
