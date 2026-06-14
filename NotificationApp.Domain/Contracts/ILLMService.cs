
namespace NotificationApp.Domain.Contracts
{
    public interface ILLMService
    {
        Task<string> GenerateMsgAsync(string level,string notificationMessage);
    }
}
