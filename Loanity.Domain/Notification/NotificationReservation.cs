using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Notification
{
    public class NotificationReservation
    {
        public int Pending { get; set; }
        public int Expired { get; set; }
        public int Active { get; set; }
        public int Total => Pending + Expired + Active;
    }
}
