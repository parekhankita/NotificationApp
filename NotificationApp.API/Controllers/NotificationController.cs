using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationApp.Domain;

namespace NotificationApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("ProcessNotification")]
        public async Task<ActionResult<NotificationResponse>> ProcessNotification([FromBody] NotificationRequest request)
        {
            return await _mediator.Send(request);
        }
    }
}
