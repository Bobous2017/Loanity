using Loanity.Domain.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos.CategoryType
{
    public record EquipmentCreateDto( //   Record EquipmentCreateDto
        string Name,
        string SerialNumber,
        string QrCode,
        EquipmentStatus Status,
        int CategoryId);
}
