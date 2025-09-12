using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Statuses
{
    public enum EquipmentStatus { Available, Loaned, Reserved, Damaged, Lost }
    public enum ReservationStatus { Pending, Active, Cancelled, Expired, Fulfilled }
    public enum LoanStatus { Active, Overdue, Closed }

}
