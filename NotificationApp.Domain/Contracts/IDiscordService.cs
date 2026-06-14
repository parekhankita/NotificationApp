namespace NotificationApp.Domain.Contracts
{
    public interface IDiscordService
    {
        Task SendMsgAsync(string message);
    }
}
