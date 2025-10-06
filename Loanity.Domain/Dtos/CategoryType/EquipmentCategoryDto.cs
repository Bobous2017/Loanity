using Loanity.Domain.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos.CategoryType
{
    public class EquipmentCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Icon { get; set; } = "box"; // key used by your CSS / font icons
    }

}
