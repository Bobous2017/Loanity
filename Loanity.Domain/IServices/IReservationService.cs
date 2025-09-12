using Loanity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.IServices
{
    public interface IReservationService
    {
        Task<Reservation> CreateAsync(int userId, int equipmentId, DateTime startAt, DateTime endAt);

    }
}
