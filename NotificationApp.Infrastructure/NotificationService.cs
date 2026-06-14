using Microsoft.Extensions.Logging;
using NotificationApp.Domain;
using NotificationApp.Domain.Contracts;

namespace NotificationApp.Infrastructure
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly ILLMService _llmService;
        private readonly IDiscordService _discordService;

        // tracking fields
        private static int _msgSentCounter = 0;
        private static DateTime _windowStartTime = DateTime.UtcNow;
        private static readonly object _lockObject = new object();

        public NotificationService(ILogger<NotificationService> logger, ILLMService llmService, IDiscordService discordService)
        {
            _llmService = llmService;
            _logger = logger;
            _discordService = discordService;
        }

        public async Task<NotificationResponse> ProcessAsync(NotificationRequest request)
        {
            try
            {
                // Info message pass without rate limiting
                if (!IsWarningOrHigher(request.Level))
                {
                    return new NotificationResponse
                    {
                        Success = true,
                        Message = $"Notification with level '{request.Level}' processed but ignored."
                    };
                }

                // Thread-Safe Rate Limit check
                lock (_lockObject)
                {
                    var currentTime = DateTime.UtcNow;

                    // If a full minute or more has passed since tracking window started, reset it
                    if (currentTime - _windowStartTime >= TimeSpan.FromMinutes(1))
                    {
                        _msgSentCounter = 0;
                        _windowStartTime = currentTime;
                        _logger.LogInformation("Rate limiting window reset. Counter cleared.");
                    }

                    // Stop if 10 alerts have already been processed in this window
                    if (_msgSentCounter >= 10)
                    {
                        _logger.LogWarning("Notification dropped: Rate limit of 10 messages per minute reached.");
                        return new NotificationResponse
                        {
                            Success = false,
                            Message = "Rate limit reached. Maximum of 10 notifications per minute allowed."
                        };
                    }

                    // Log the slot claim and increment the count
                    _msgSentCounter++;
                    _logger.LogInformation($"Rate limit permit claimed. Current minute usage: {_msgSentCounter}/10");
                }
               
                var message = await _llmService.GenerateMsgAsync(request.Level, request.Message);
                await _discordService.SendMsgAsync(message);

                return new NotificationResponse
                {
                    Success = true,
                    Message = "Notification sent successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing notification.");

                return new NotificationResponse
                {
                    Success = false,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }
        private static bool IsWarningOrHigher(string level)
        {
            return level.Equals("Warning", StringComparison.OrdinalIgnoreCase)
                   || level.Equals("Error", StringComparison.OrdinalIgnoreCase)
                   || level.Equals("Critical", StringComparison.OrdinalIgnoreCase);
        }
    }
}