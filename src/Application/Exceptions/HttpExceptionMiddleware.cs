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

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next.Invoke(context);
            }
            catch (HttpException httpException)
            {
                context.Response.StatusCode = httpException.StatusCode;
                var responseFeature = context.Features.Get<IHttpResponseFeature>();
                responseFeature.ReasonPhrase = httpException.Message;

                // El front necesita body JSON; ReasonPhrase solo no llega a fetch().
                if (!context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json; charset=utf-8";
                    var payload = Encoding.UTF8.GetBytes(
                        $"{{\"message\":{System.Text.Json.JsonSerializer.Serialize(httpException.Message ?? "")}}}");
                    await context.Response.Body.WriteAsync(payload);
                }
            }
        }
    }
}
