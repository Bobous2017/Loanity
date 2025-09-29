using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos.UserHandlingDto
{
    public class UserLoanDto
    {
        public int LoanId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        public string EquipmentName { get; set; } = string.Empty;

        public DateTime StartAt { get; set; }
        public DateTime DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }

        public string Status { get; set; } = string.Empty;
    }

}
