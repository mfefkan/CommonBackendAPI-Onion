using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ResendMailService : IMailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ResendMailService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var apiKey = _configuration["Resend:ApiKey"];
            var from = _configuration["Resend:From"]; 

            var payload = new
            {
                from,
                to = new[] { to },
                subject,
                html = htmlBody
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Cannot send: {msg}");
            }
        }
    }
}
