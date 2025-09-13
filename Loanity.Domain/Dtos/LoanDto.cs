using Loanity.Domain.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos
{
    public record LoanDto(
        int Id,
        int UserId,
        DateTime StartAt,
        DateTime DueAt,
        DateTime? ReturnedAt,
        string Status, // This is the key: accept enum as string
        int? ReservationId,
        List<LoanItemDto> Items
    );

}
