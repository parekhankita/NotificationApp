namespace NotificationApp.Domain.Contracts
{
    public interface INotificationService
    {
        Task<NotificationResponse> ProcessAsync(NotificationRequest request);
    }
}
