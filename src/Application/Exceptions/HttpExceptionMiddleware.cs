using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    internal class HttpExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public HttpExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        private static bool IsDevelopment()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task WriteJsonError(HttpContext context, int statusCode, string message)
        {
            if (context.Response.HasStarted)
            {
                throw new InvalidOperationException(
                    "The response has already started, cannot write exception body.");
            }

            context.Response.StatusCode = statusCode;
            var responseFeature = context.Features.Get<IHttpResponseFeature>();
            if (responseFeature != null)
            {
                responseFeature.ReasonPhrase = message;
            }

            context.Response.ContentType = "application/json; charset=utf-8";
            var payload = Encoding.UTF8.GetBytes(
                $"{{\"message\":{System.Text.Json.JsonSerializer.Serialize(message ?? "")}}}");
            await context.Response.Body.WriteAsync(payload);
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next.Invoke(context);
            }
            catch (HttpException httpException)
            {
                await WriteJsonError(context, httpException.StatusCode, httpException.Message);
            }
            catch (Exception)
            {
                // Development: let UseDeveloperExceptionPage surface the real error.
                // Staging/Production: JSON 500 (never empty body / remapped 404).
                if (IsDevelopment())
                {
                    throw;
                }

                await WriteJsonError(context, StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }
    }
}
