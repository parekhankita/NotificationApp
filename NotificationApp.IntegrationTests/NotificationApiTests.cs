using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NotificationApp.Domain;
using NotificationApp.Domain.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace NotificationApp.IntegrationTests
{
    public class NotificationApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public NotificationApiTests(WebApplicationFactory<Program> factory)
        {
            // Injecting Mocks 
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var mockLlm = new Mock<ILLMService>();
                    mockLlm.Setup(x => x.GenerateMsgAsync(It.IsAny<string>(), It.IsAny<string>()))
                           .ReturnsAsync("Mocked LLM Alert Message");

                    var mockDiscord = new Mock<IDiscordService>();
                    mockDiscord.Setup(x => x.SendMsgAsync(It.IsAny<string>()))
                               .Returns(Task.CompletedTask);

                    services.AddScoped(_ => mockLlm.Object);
                    services.AddScoped(_ => mockDiscord.Object);
                });
            }).CreateClient();
        }

        [Fact]
        public async Task Should_Only_Enforce_Rate_Limit_On_Sent_Warning_And_Error_Messages()
        {
            // Arrange: 
            var infoData = new NotificationRequest
            {
                Source = "UnitTest",
                Level = "info",
                Message = "System check."
            };

            var warningData = new NotificationRequest
            {
                Source = "PaymentService",
                Level = "warning",
                Message = "Database connection failed."
            };

            var errorData = new NotificationRequest
            {
                Source = "OrderService",
                Level = "error",
                Message = "Out of memory exception!"
            };

            // Act: Send 15 "Info" requests
            for (int i = 0; i < 15; i++)
            {
                // Fixed typo here: passing infoData safely
                var response = await _client.PostAsJsonAsync("/api/Notification/ProcessNotification", infoData);
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var content = await response.Content.ReadFromJsonAsync<NotificationResponse>();
                content.Should().NotBeNull();
                content!.Success.Should().BeTrue();
                content.Message.Should().Contain("ignored");
            }

            // Act: Send a mix of 5 Warnings and 5 Errors 
            for (int i = 0; i < 5; i++)
            {
                // Send Warning
                var warnResponse = await _client.PostAsJsonAsync("/api/Notification/ProcessNotification", warningData);
                warnResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                var warnContent = await warnResponse.Content.ReadFromJsonAsync<NotificationResponse>();
                warnContent!.Success.Should().BeTrue();

                // Send Error
                var errorResponse = await _client.PostAsJsonAsync("/api/Notification/ProcessNotification", errorData);
                errorResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                var errorContent = await errorResponse.Content.ReadFromJsonAsync<NotificationResponse>();
                errorContent!.Success.Should().BeTrue();
            }

            // Act: Send the 11th high-priority request 
            var blockedResponse = await _client.PostAsJsonAsync("/api/Notification/ProcessNotification", errorData);
            blockedResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var blockedContent = await blockedResponse.Content.ReadFromJsonAsync<NotificationResponse>();
            blockedContent.Should().NotBeNull();

            // Assert: The rate limit reached
            blockedContent!.Success.Should().BeFalse();
            blockedContent.Message.Should().Contain("Rate limit reached");
        }
    }
}