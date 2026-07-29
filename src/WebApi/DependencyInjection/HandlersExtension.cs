using Application.CommandHandlers;
using Application.Common;
using Application.Dtos;
using Application.Services.Comandos;
using Application.Services.Validaciones;
using Application.Validators;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.DependencyInjection
{
    public static class HandlersExtension
    {
        public static void AddHandlers(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetCatalogHandler).Assembly);
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Application.Behaviors.ValidationBehavior<,>));
            services.AddTransient<ICommandBus, CommandBus>();
            services.AddTransient<IValidatorService, ValidatorService>();

            services.Scan(scan => scan
                .FromAssemblyOf<IValidator>()
                .AddClasses(classes => classes.AssignableTo<IValidator>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }
    }
}
