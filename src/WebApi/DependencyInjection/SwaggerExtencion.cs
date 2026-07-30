using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace WebApi.DependencyInjection
{
    public static class SwaggerExtencion
    {
        public static void AddSwaggerConf(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("alpha", new OpenApiInfo
                {
                    Version = "alpha",
                    Title = "Corelux Auth API",
                    Description = "Authentication, users, roles and permissions.",
                    Contact = new OpenApiContact
                    {
                        Name = "Corelux",
                        Email = "",
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
