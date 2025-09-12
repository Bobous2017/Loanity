using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos.ScanType
{
    // Role Request input
    public record ScanRequestDto
    (
        int UserId, 
        string QrCode, 
        DateTime? DueAt, 
        string Action
        
    );
}
