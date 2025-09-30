using Loanity.Domain.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Entities
{
    public class Loan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartAt { get; set; } = DateTime.UtcNow;
        public DateTime DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Active;
        public int? ReservationId { get; set; }    // optional link
        public ICollection<LoanItem> Items { get; set; } = new List<LoanItem>();
        public User User { get; set; } = null!;
    }
}
