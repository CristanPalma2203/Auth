using Aplicacion.CommandHandlers;
using Aplicacion.Common;
using Aplicacion.Dtos;
using Aplicacion.Services.Comandos;
using Aplicacion.Services.Validaciones;
using Aplicacion.Validators;
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
                cfg.RegisterServicesFromAssembly(typeof(ConsultarCatalogoHandler).Assembly);
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Aplicacion.Behaviors.ValidationBehavior<,>));
            services.AddTransient<ICommandBus, CommandBus>();
            services.AddTransient<IValidatorService, ValidatorService>();

            services.Scan(scan => scan
                .FromAssemblyOf<IValidador>()
                .AddClasses(classes => classes.AssignableTo<IValidador>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }
    }
}
