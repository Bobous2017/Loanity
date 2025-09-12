using Loanity.Domain.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EquipmentId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    }
}
