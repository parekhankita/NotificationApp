using Microsoft.Extensions.Configuration;
using NotificationApp.Domain.Contracts;
using System.Net.Http.Json;

namespace NotificationApp.Infrastructure
{
    public class DiscordService : IDiscordService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public DiscordService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        // it sends message to discord webhook url 
        public async Task SendMsgAsync(string message)
        {
            //option -1: read from configuration
            var webhookUrl = _configuration["Discord:WebhookUrl"];

            //option -2: Direct pass the webhook url as parameter which is not secure and not recommended
            //var webhookUrl = "https://discord.com/api/webhooks/xxxxxx";

            if (string.IsNullOrEmpty(webhookUrl))
            {
                throw new Exception("Discord webhook is not found");
            }

            var data = new { content = message };

            var response = await _httpClient.PostAsJsonAsync(webhookUrl, data);
            response.EnsureSuccessStatusCode();
        }
    }
}