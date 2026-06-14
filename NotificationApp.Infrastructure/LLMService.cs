using Microsoft.Extensions.Configuration;
using NotificationApp.Domain.Contracts;

namespace NotificationApp.Infrastructure
{
    public class LLMService : ILLMService
    {
       // private readonly HttpClient _httpClient;
        //private readonly IConfiguration _configuration;

        public LLMService(HttpClient httpClient, IConfiguration configuration)
        {
           // _httpClient = httpClient;
            //_configuration = configuration;
        }
        // For Demo Purpose
        public Task<string> GenerateMsgAsync(string level, string message)
        {
            var response = $"""
                            🚨 {level.ToUpper()} ALERT

                                Issue:
                                {message}

                                Analysis:
                                This appears to be a {level} level system issue.

                                Recommended Action:
                                Please investigate logs and affected services.
                            """;

            return Task.FromResult(response);
        }
        // Requires billing or else throws exception releated to limited Quota

        //public async Task<string> GenerateMsgAsync(string level, string message)
        //{
        //    var apiKey = _configuration["OpenAI:ApiKey"];
        //    var model = _configuration["OpenAI:Model"];

        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        //    var prompt = $@"
        //                        Analyze the following notification.

        //                        Level: {level}
        //                        Message: {message}

        //                        Determine:
        //                        1. What type of issue is this?
        //                        2. Severity of the issue
        //                        3. Recommended action to perform

        //                        Generate a concise alert suitable for Discord.
        //                        ";

        //    var requestBody = new
        //    {
        //        model,messages = new[]     {
        //        new
        //        {
        //            role = "user",
        //            content = prompt
        //        }
        //    },
        //        temperature = 0.3
        //    };

        //    var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions",requestBody);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var error = await response.Content.ReadAsStringAsync();
        //        throw new Exception($"OpenAI Error: {response.StatusCode} - {error}");
        //    }

        //    var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        //    return result
        //        .GetProperty("choices")[0]
        //        .GetProperty("message")
        //        .GetProperty("content")
        //        .GetString();
        //}
    }
}

