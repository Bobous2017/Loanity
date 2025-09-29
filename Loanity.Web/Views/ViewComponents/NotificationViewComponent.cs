using Loanity.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.Web.Views.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly NotificationService _client;
        public NotificationViewComponent(NotificationService client)
        {
            _client = client;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var summary = await _client.GetNotificationReservationSummaryAsync();
            return View(summary);
        }
    }

}
