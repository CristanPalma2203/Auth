using Infraestructura.Service;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.DependencyInjection
{
    public static class MailExtension
    {
        public static void AddMail(this IServiceCollection services)
        {
            services.AddSingleton<ResendEmailClient>();
        }
    }
}
