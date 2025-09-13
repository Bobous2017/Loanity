using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Statuses
{
    public enum EquipmentStatus { Available, Loaned, Reserved, Damaged, Lost }
    public enum ReservationStatus { Pending = 0, Active = 1, Cancelled= 2, Expired=3, Fulfilled=4 }
    public enum LoanStatus { Active, Overdue, Closed}

}
