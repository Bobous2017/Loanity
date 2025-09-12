using Loanity.Domain.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Entities
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public EquipmentCategory? Category { get; set; }
        public string SerialNumber { get; set; } = null!;
        public string QrCode { get; set; } = null!;
        public string? Color { get; set; }
        public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;
        public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();

    }
}
