using Loanity.Domain.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos.CategoryType
{
    public record EquipmentUpdateDto( //   Record EquipmentUpdateDto
           int Id,
           string Name,
           string SerialNumber,
           string QrCode,
           EquipmentStatus Status,
           int CategoryId
       );
}
