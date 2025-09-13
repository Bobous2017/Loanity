using Loanity.Domain.Entities;

namespace Loanity.Domain.IServices
{
    public interface IReservationService
    {
        //Loan Business rules  ( Statuses for Reservation to tryggere Equipment ) ----------------------------------1
        // Create a new reservation (Pending)
        Task<Reservation> CreateAsync(int userId, int equipmentId, DateTime startAt, DateTime endAt);

        // Update a reservation (New or old Statut)
        Task<bool> UpdateAsync(Reservation updated);

        // Activate reservation (e.g., time window has started)
        Task<bool> ActivateReservationAsync(int reservationId);

        // Mark as fulfilled (equipment picked up by user)
        Task<bool> FulfillReservationAsync(int reservationId);

        // Cancel reservation (by user or system)
        Task<bool> CancelReservationAsync(int reservationId);

        // Expire reservation (time window passed, no pickup)
        Task<bool> ExpireReservationAsync(int reservationId);
    }
}
