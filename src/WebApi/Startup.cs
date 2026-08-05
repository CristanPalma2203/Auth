using Application.Exceptions;
using Application.Mappers;
using Domain.Helpers;
using Domain.Models.Rules;
using Domain.Service;
using Infrastructure.Filters;
using Infrastructure.Service;
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
            services.AddApplicationServices();
            services.AddTokenConfiguration(Configuration);
            services.AddHttpContextAccessor();
            services.AddRedis(Configuration);
            services.AddCorsConfig(Configuration);
            services.AddSwaggerConf();
            services.AddHealthChecks();

            services.Scan(scan => scan
                .FromAssemblyOf<CambioPassword>()
                .AddClasses(classes => classes.AssignableTo(typeof(IRule)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            services.AddTransient<IEmailHelper, EmailHelper>();
            services.AddScoped<ITenantLookup, TenantLookup>();
            services.AddMail();
            services.AddHttpClient();

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

            app.UseRouting();
            // CORS antes de endpoints/exception body → browser ve 500 real, no TypeError falso
            app.UseCors("ApiCorsPolicy");
            app.UseHttpException();
            app.UseSwagger(c =>
            {
                // Swagger 2.0 avoids openapi:3.0.4 UI parse issues with Microsoft.OpenApi 1.6.25+
                c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/alpha/swagger.json", "Corelux Auth API");
                c.DocumentTitle = "Corelux Auth API";
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
