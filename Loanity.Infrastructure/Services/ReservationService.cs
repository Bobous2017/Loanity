using Loanity.Domain.Entities;
using Loanity.Domain.IServices;
using Loanity.Domain.Statuses;
using Microsoft.EntityFrameworkCore;

namespace Loanity.Infrastructure.Services
{
    public class ReservationService : IReservationService
    {
        private readonly LoanityDbContext _db;
        private readonly IEmailService _email;
        public ReservationService(LoanityDbContext db, IEmailService email)
        {
            _db = db;
            _email = email;
        }

        public async Task<Reservation> CreateAsync(int userId, int equipmentId, DateTime startAt, DateTime endAt)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR: " + ex.Message); // or use ILogger
                throw;
            }
        }


        
        public async Task<bool> UpdateAsync(Reservation updated)
        {
            var reservation = await _db.Reservations
                .Include(r => r.Equipment)
                .Include(r => r.User) // 👈 Make sure we have access to the user's email
                .FirstOrDefaultAsync(r => r.Id == updated.Id);

            if (reservation == null)
                return false;

            // Update fields
            reservation.StartAt = updated.StartAt;
            reservation.EndAt = updated.EndAt;
            reservation.UserId = updated.UserId;
            reservation.EquipmentId = updated.EquipmentId;
            reservation.Status = updated.Status;

            // Prepare dynamic values for email
            var toEmail = reservation.User.Email; // 👈 REAL user email
            var equipmentName = reservation.Equipment?.Name ?? "Unknown Equipment";
            var qrCodeText = reservation.Equipment?.QrCode ?? "UNKNOWN_QR";

            var startAt = reservation.StartAt;
            var endAt = reservation.EndAt;

            // Update Equipment status
            var equipment = await _db.Equipment.FindAsync(updated.EquipmentId);
            if (equipment != null)
            {
                switch (updated.Status)
                {
                    case ReservationStatus.Active:
                        equipment.Status = EquipmentStatus.Reserved;

                        var qrActive = new GenerateQRCode();
                        using (var stream = qrActive.GenerateQRCodeGen(qrCodeText,reservation.UserId, null))
                        {
                            var activeBody = $@"
                        <h3>Your Reservation is Active</h3>
                        <p>Equipment: <b>{equipmentName}</b></p>
                        <p>Pickup Location: Loanity Equipment Building 8</p>
                        <p>From: {startAt}</p>
                        <p>To: {endAt}</p>
                        <p>Show the attached QR code when picking up your equipment.</p>";

                            await _email.SendAsync(
                                toEmail,
                                $"Reservation Confirmed: {equipmentName}",
                                activeBody,
                                stream,
                                "reservation_qr.png"
                            );
                        }
                        break;

                    case ReservationStatus.Fulfilled:
                        equipment.Status = EquipmentStatus.Loaned;

                        var qrFulfilled = new GenerateQRCode();
                        using (var stream = qrFulfilled.GenerateQRCodeGen(qrCodeText,reservation.UserId, null))
                        {
                            var fulfilledBody = $@"
                        <h3>Loan Confirmed</h3>
                        <p>Your reservation has been fulfilled and is now a loan.</p>
                        <p><b>Equipment:</b> {equipmentName}</p>
                        <p><b>From:</b> {startAt}</p>
                        <p><b>To:</b> {endAt}</p>
                        <p>Reference: LOAN-{reservation.Id}</p>";

                            await _email.SendAsync(
                                toEmail,
                                $"Loan Confirmed: {equipmentName}",
                                fulfilledBody,
                                stream,
                                "reservation_qr.png"
                            );
                        }
                        break;

                    case ReservationStatus.Cancelled:
                        equipment.Status = EquipmentStatus.Available;

                        var cancelBody = $@"
                    <h3>Reservation Cancelled</h3>
                    <p>Your reservation was cancelled or rejected.</p>
                    <p><b>Equipment:</b> {equipmentName}</p>
                    <p><b>Reservation ID:</b> {reservation.Id}</p>";

                        await _email.SendAsync(
                            toEmail,
                            "Reservation Cancelled",
                            cancelBody
                        );
                        break;

                    case ReservationStatus.Expired:
                        equipment.Status = EquipmentStatus.Available;
                        break;
                }
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
