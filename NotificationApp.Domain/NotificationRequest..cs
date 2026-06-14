using MediatR;

namespace NotificationApp.Domain
{
    public class NotificationRequest : IRequest<NotificationResponse>

    {
        public string Source { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }
    }
}
