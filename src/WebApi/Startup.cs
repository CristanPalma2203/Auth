using Aplicacion.Exceptions;
using Aplicacion.Mappers;
using Dominio.Helpers;
using Dominio.Models.Regla;
using Infraestructura.Filters;
using Infraestructura.Service;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.DependencyInjection;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var mapsterConfig = TypeAdapterConfig.GlobalSettings;
            mapsterConfig.Scan(typeof(MappingConfig).Assembly);
            services.AddSingleton(mapsterConfig);
            services.AddScoped<IMapper, ServiceMapper>();

            services.AddHandlers();
            services.AddContextConfiguration(Configuration);
            services.AddScoped<UnitOfWordFilter>();
            services.AddAplicacionServices();
            services.AddTokenConfiguration(Configuration);
            services.AddHttpContextAccessor();
            services.AddRedis(Configuration);
            services.AddCorsConfig();
            services.AddSwaggerConf();

            services.Scan(scan => scan
                .FromAssemblyOf<CambioPassword>()
                .AddClasses(classes => classes.AssignableTo(typeof(IRegla)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            services.AddTransient<ICorreoHelper, CorreoHelper>();
            services.AddMail();

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(UnitOfWordFilter));
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpException();
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/alpha/swagger.json", "Documentacion Sistema de Pagos");
            });
            app.UseCors("ApiCorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
