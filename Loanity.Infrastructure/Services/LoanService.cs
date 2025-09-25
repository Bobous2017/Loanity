using Loanity.Domain.Entities;
using Loanity.Domain.IServices;
using Loanity.Domain.Statuses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Infrastructure.Services
{
    //Loan logic( Inherite ILoanService and  Implimentation and Sync to Db )----------------2
    public class LoanService : ILoanService
    {
        private readonly LoanityDbContext _db;
        public LoanService(LoanityDbContext db) => _db = db;

        //public async Task<Loan> CreateLoanFromScanAsync(int userId, string qrCode, DateTime dueAt)
        //{
        //    var item = await _db.Equipment.SingleOrDefaultAsync(e => e.QrCode == qrCode);
        //    if (item is null) throw new InvalidOperationException("Device not found");

        //    //if (item.Status is not EquipmentStatus.Available)
        //    //    throw new InvalidOperationException("Device not available");

        //    if (item.Status != EquipmentStatus.Available && item.Status != EquipmentStatus.Reserved)
        //        throw new InvalidOperationException("Device not available");


        //    var loan = new Loan { UserId = userId, DueAt = dueAt,  Status = LoanStatus.Active };
        //    loan.Items.Add(new LoanItem { Loan = loan, Equipment = item });

        //    item.Status = EquipmentStatus.Loaned;

        //    _db.Loans.Add(loan);
        //    await _db.SaveChangesAsync();
        //    return loan;
        //}
        public async Task<Loan> CreateLoanFromScanAsync(int userId, string qrCode, DateTime dueAt)
        {
            var item = await _db.Equipment.SingleOrDefaultAsync(e => e.QrCode == qrCode);
            if (item is null) throw new InvalidOperationException("Device not found");

            // Tillad både Available and Reserved status
            if (item.Status != EquipmentStatus.Available && item.Status != EquipmentStatus.Reserved)
                throw new InvalidOperationException("Device not available");

            // Hvis den er reservere, check if the user is the one who reserved it 
            if (item.Status == EquipmentStatus.Reserved)
            {
                var reservation = await _db.Reservations
                    .FirstOrDefaultAsync(r => r.EquipmentId == item.Id && r.UserId == userId && r.Status == ReservationStatus.Active);

                if (reservation is null)
                    throw new InvalidOperationException("You did not reserve this item.");

                // Reservation bliver fulfilled  fordi  Equipement Status blive loaned
                reservation.Status = ReservationStatus.Fulfilled;
            }

            var loan = new Loan { UserId = userId, DueAt = dueAt, Status = LoanStatus.Active };
            loan.Items.Add(new LoanItem { Loan = loan, Equipment = item });

            item.Status = EquipmentStatus.Loaned;

            _db.Loans.Add(loan);
            await _db.SaveChangesAsync();
            return loan;
        }

        public async Task<Loan?> ReturnByScanAsync(int userId, string qrCode)
        {
            var item = await _db.Equipment.SingleOrDefaultAsync(e => e.QrCode == qrCode);
            if (item is null) return null;

            // find active loan containing this item
            var loan = await _db.Loans
                .Include(l => l.Items)
                .Where(l => l.Status == LoanStatus.Active && l.Items.Any(i => i.EquipmentId == item.Id))
                .OrderByDescending(l => l.StartAt)
                .FirstOrDefaultAsync();

            if (loan is null) return null;

            loan.ReturnedAt = DateTime.UtcNow;
            loan.Status = LoanStatus.Closed;
            item.Status = EquipmentStatus.Available;

            await _db.SaveChangesAsync();
            return loan;
        }
      

    }


}
