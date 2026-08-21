using System;
using Domain.Repositories;
using Domain.Service;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace WebApi.DependencyInjection
{
    public static class ContextrExtensions
    {
        public static void AddContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<AutenticationContext>(
         options =>
         {
             options.UseSqlServer(
                 configuration.GetConnectionString("DefaultConnection"),
                 sql => sql.EnableRetryOnFailure(
                     maxRetryCount: 5,
                     maxRetryDelay: TimeSpan.FromSeconds(30),
                     errorNumbersToAdd: null));

            // options.EnableSensitiveDataLogging();
         });

            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.Scan(scan => scan.FromAssemblyOf<AppUserRepository>().AddClasses(classes => classes.AssignableTo(typeof(IGenericRepository<>))).AsImplementedInterfaces().WithScopedLifetime());


            // services.AddScoped<IAppUserRepository, AppUserRepository>();


        }
    }
}
