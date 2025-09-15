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
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int UserId { get; set; }
        public int EquipmentId { get; set; }
        public int? LoanId { get; set; } // 

        public ReservationStatus Status { get; set; }

        public Equipment? Equipment { get; set; }
        public User? User { get; set; }
        public Loan? Loan { get; set; }
    }

}
