using Loanity.Domain.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos
{
   
    public record EquipmentDto
    (
        int Id, 
        string Name, 
        string SerialNumber, 
        string QrCode, 
        EquipmentStatus Status, 
        int CategoryId
    );
   
}
