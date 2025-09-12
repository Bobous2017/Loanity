using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Entities
{
    public class LoanItem
    {
        public int LoanId { get; set; }
        public int EquipmentId { get; set; }
        public Loan Loan { get; set; } = null!;
        public Equipment Equipment { get; set; } = null!;

    }
}
