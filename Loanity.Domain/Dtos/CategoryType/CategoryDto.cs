using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos.CategoryType
{
    public record CategoryDto
    (
        int Id,
        string Name,
         int Quantity,
        string Icon = "box" // key used by your CSS / font icons
    );
}
