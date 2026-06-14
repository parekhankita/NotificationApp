using MediatR;
using NotificationApp.Domain;
using NotificationApp.Domain.Contracts;

namespace NotificationApp.Application
{  
    // uses of the mediator pattern to seperate the business logic
    public class ProcessNotificationHandler : IRequestHandler<NotificationRequest, NotificationResponse>
    {
        private readonly INotificationService _notificationService;
        public ProcessNotificationHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<NotificationResponse> Handle(NotificationRequest request,CancellationToken cancellationToken)
        {
            try
            {
                return await _notificationService.ProcessAsync(request);
            }
            catch (Exception ex)
            {
                return new NotificationResponse
                {
                    Success = false,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }
    }
}
