using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos
{
    public class CategorySummaryDto
    {
        public int Total { get; set; }
        public int Available { get; set; }
        public int Reserved { get; set; }
        public int Loaned { get; set; }
        public List<EquipmentDto> Equipment { get; set; }
    }

}
