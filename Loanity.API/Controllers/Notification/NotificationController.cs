using Loanity.Domain.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.API.Controllers.Notification
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var summary = await _service.GetNotificationReservationSummaryAsync();
            return Ok(summary);
        }
    }

}
