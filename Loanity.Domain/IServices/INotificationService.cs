using Loanity.Domain.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.IServices
{
    public interface INotificationService
    {
        Task<NotificationReservation> GetNotificationReservationSummaryAsync();
    }

   

}
