using Loanity.Domain.Entities;
using Loanity.Domain.IServices;
using Loanity.Domain.Statuses;
using Microsoft.EntityFrameworkCore;

namespace Loanity.Infrastructure.Services
{
    public class ReservationService : IReservationService
    {
        private readonly LoanityDbContext _db;
        public ReservationService(LoanityDbContext db) => _db = db;

        // Create a reservation (Pending)
        public async Task<Reservation> CreateAsync(int userId, int equipmentId, DateTime startAt, DateTime endAt)
        {
            var equipment = await _db.Equipment.FindAsync(equipmentId);
            if (equipment == null) throw new InvalidOperationException("Equipment not found");
            if (equipment.Status != EquipmentStatus.Available)
                throw new InvalidOperationException("Equipment is not available for reservation");

            var reservation = new Reservation
            {
                UserId = userId,
                EquipmentId = equipmentId,
                StartAt = startAt,
                EndAt = endAt,
                Status = ReservationStatus.Pending
            };

            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync();
            return reservation;
        }

        // Update a reservation (New or old Statut)
        public async Task<bool> UpdateAsync(Reservation updated)
        {
            var reservation = await _db.Reservations
                .Include(r => r.Equipment) // 👈 Important
                .FirstOrDefaultAsync(r => r.Id == updated.Id);

            if (reservation == null)
                return false;

            reservation.StartAt = updated.StartAt;
            reservation.EndAt = updated.EndAt;
            reservation.UserId = updated.UserId;
            reservation.EquipmentId = updated.EquipmentId;
            reservation.Status = updated.Status;

            // 🚨 Business rule: Update Equipment status based on Reservation status
            var equipment = await _db.Equipment.FindAsync(updated.EquipmentId);
            if (equipment != null)
            {
                equipment.Status = updated.Status switch
                {
                    ReservationStatus.Active => EquipmentStatus.Reserved,
                    ReservationStatus.Fulfilled => EquipmentStatus.Loaned,
                    ReservationStatus.Cancelled => EquipmentStatus.Available,
                    ReservationStatus.Expired => EquipmentStatus.Available,
                    _ => equipment.Status
                };
            }

            await _db.SaveChangesAsync();
            return true;
        }

        // Activate a reservation (make it valid now)
        public async Task<bool> ActivateReservationAsync(int reservationId)
        {
            var reservation = await _db.Reservations.Include(r => r.Equipment).FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null || reservation.Status != ReservationStatus.Pending)
                return false;

            reservation.Status = ReservationStatus.Active;
            reservation.Equipment.Status = EquipmentStatus.Reserved;

            await _db.SaveChangesAsync();
            return true;
        }

        // Fulfill (picked up)
        public async Task<bool> FulfillReservationAsync(int reservationId)
        {
            var reservation = await _db.Reservations.Include(r => r.Equipment).FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null || reservation.Status != ReservationStatus.Active)
                return false;

            reservation.Status = ReservationStatus.Fulfilled;
            reservation.Equipment.Status = EquipmentStatus.Loaned;

            await _db.SaveChangesAsync();
            return true;
        }

        // Cancel
        public async Task<bool> CancelReservationAsync(int reservationId)
        {
            var reservation = await _db.Reservations.Include(r => r.Equipment).FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null || reservation.Status == ReservationStatus.Cancelled || reservation.Status == ReservationStatus.Fulfilled)
                return false;

            reservation.Status = ReservationStatus.Cancelled;
            reservation.Equipment.Status = EquipmentStatus.Available;

            await _db.SaveChangesAsync();
            return true;
        }

        // Expire
        public async Task<bool> ExpireReservationAsync(int reservationId)
        {
            var reservation = await _db.Reservations.Include(r => r.Equipment).FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null || reservation.Status != ReservationStatus.Pending)
                return false;

            reservation.Status = ReservationStatus.Expired;
            reservation.Equipment.Status = EquipmentStatus.Available;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
