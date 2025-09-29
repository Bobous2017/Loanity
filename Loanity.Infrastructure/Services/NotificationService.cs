using Loanity.Domain.IServices;
using Loanity.Domain.Notification;
using Loanity.Domain.Statuses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loanity.Domain.IServices;
namespace Loanity.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly LoanityDbContext _db;
        public NotificationService(LoanityDbContext db) => _db = db;

        public async Task<NotificationReservation> GetNotificationReservationSummaryAsync()
        {
            var pending = await _db.Reservations.CountAsync(r => r.Status == ReservationStatus.Pending);
            var expired = await _db.Reservations.CountAsync(r => r.Status == ReservationStatus.Expired);
            var active = await _db.Reservations.CountAsync(r => r.Status == ReservationStatus.Active);

            return new NotificationReservation
            {
                Pending = pending,
                Expired = expired,
                Active = active
            };
        }

    }
}
