using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos
{
    public record LoanItemDto(
         int EquipmentId,
         string? EquipmentName,
         string? QrCode
     );
}
