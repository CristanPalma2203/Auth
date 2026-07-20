using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infraestructura.Service
{
    /// <summary>
    /// Cliente HTTP para Resend (https://api.resend.com/emails).
    /// ApiKey: Email:Resend:ApiKey o variable de entorno Email__Resend__ApiKey.
    /// </summary>
    public class ResendEmailClient
    {
        private static readonly HttpClient Http = new HttpClient
        {
            BaseAddress = new Uri("https://api.resend.com/")
        };

        private readonly IConfiguration configuration;

        public ResendEmailClient(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Send(string to, string subject, string html)
        {
            SendAsync(to, subject, html).GetAwaiter().GetResult();
        }

        public async Task SendAsync(string to, string subject, string html)
        {
            var apiKey = configuration["Email:Resend:ApiKey"]
                ?? Environment.GetEnvironmentVariable("Email__Resend__ApiKey");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "Falta Email:Resend:ApiKey. Configúrala con user-secrets o la variable Email__Resend__ApiKey.");
            }

            var from = configuration["Email:From"] ?? "ERP Base <onboarding@resend.dev>";

            using var request = new HttpRequestMessage(HttpMethod.Post, "emails");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var payload = new
            {
                from,
                to = new[] { to },
                subject,
                html
            };
            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await Http.SendAsync(request).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Resend error {(int)response.StatusCode}: {body}");
            }
        }
    }
}
