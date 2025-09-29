using Loanity.Domain.Notification;

namespace Loanity.Web.Services
{
    public class NotificationService
    {
        private readonly HttpClient _http;

        public NotificationService(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("LoanityApi");
        }

        public async Task<NotificationReservation> GetNotificationReservationSummaryAsync()
        {
            return await _http.GetFromJsonAsync<NotificationReservation>("api/notification");
        }

    }
}
