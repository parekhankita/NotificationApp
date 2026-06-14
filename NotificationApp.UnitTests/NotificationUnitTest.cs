using Microsoft.Extensions.Logging;
using Moq;
using NotificationApp.Domain;
using NotificationApp.Domain.Contracts;
using NotificationApp.Infrastructure;
public class NotificationUnitTest
{
    [Fact]
    public async Task Error_Level_Should_Send_Discord_Message()
    {
        // Arrange
        var llm = new Mock<ILLMService>();
        var discord = new Mock<IDiscordService>();
        var logger = new Mock<ILogger<NotificationService>>();

        llm.Setup(x => x.GenerateMsgAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("Generated Alert");

        var service = new NotificationService(logger.Object, llm.Object, discord.Object);

        var request = new NotificationRequest
        {
            Level = "error",
            Message = "Database failed"
        };

        // Act
        var result = await service.ProcessAsync(request);

        // Assert
        Assert.True(result.Success);
        discord.Verify(x => x.SendMsgAsync("Generated Alert"), Times.Once);
    }

    [Fact]
    public async Task Info_Level_Should_Not_Send_Message()
    {
        // Arrange
        var llm = new Mock<ILLMService>();
        var discord = new Mock<IDiscordService>();
        var logger = new Mock<ILogger<NotificationService>>();

        var service = new NotificationService(logger.Object, llm.Object, discord.Object);

        var request = new NotificationRequest
        {
            Level = "info",
            Message = "Application started"
        };

        // Act
        var result = await service.ProcessAsync(request);

        // Assert
        Assert.True(result.Success);
        discord.Verify(x => x.SendMsgAsync(It.IsAny<string>()), Times.Never);
    }
}