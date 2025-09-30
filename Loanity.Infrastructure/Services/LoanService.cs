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
        private readonly IEmailService _email;

        public LoanService(LoanityDbContext db, IEmailService email)
        {
            _db = db;
            _email = email;
        }


        //public async Task<Loan> CreateLoanFromScanAsync(int userId, string qrCode, DateTime dueAt)
        //{
        //    var item = await _db.Equipment.SingleOrDefaultAsync(e => e.QrCode == qrCode);
        //    if (item is null) throw new InvalidOperationException("Udstyret Findes Ikke!");

        //    //----------------------------------------------

        //    // Tillad både Available and Reserved status
        //    if (item.Status != EquipmentStatus.Available && item.Status != EquipmentStatus.Reserved)
        //        throw new InvalidOperationException("Udstyret er ikke tilgængelig!");

        //    // Hvis den er reservere, check if the user is the one who reserved it 
        //    if (item.Status == EquipmentStatus.Reserved)
        //    {
        //        var reservation = await _db.Reservations
        //            .FirstOrDefaultAsync(r => r.EquipmentId == item.Id && r.UserId == userId && r.Status == ReservationStatus.Active);

        //        if (reservation is null)
        //            throw new InvalidOperationException("Du har ikke reserveret det her udstyret.");

        //        // Reservation bliver fulfilled  fordi  Equipement Status blive loaned
        //        reservation.Status = ReservationStatus.Fulfilled;
        //    }

        //    var loan = new Loan { UserId = userId, DueAt = dueAt, Status = LoanStatus.Active };
        //    loan.Items.Add(new LoanItem { Loan = loan, Equipment = item });

        //    item.Status = EquipmentStatus.Loaned;

        //    _db.Loans.Add(loan);
        //    await _db.SaveChangesAsync();
        //    return loan;
        //}




        public async Task<Loan> CreateLoanFromScanAsync(int userId, string qrCode, DateTime dueAt)
        {
            var item = await _db.Equipment
                .Include(e => e.Reservations) // Needed to access reservation details
                .FirstOrDefaultAsync(e => e.QrCode == qrCode);

            if (item is null)
                throw new InvalidOperationException("Udstyret Findes Ikke!");

            if (item.Status != EquipmentStatus.Available && item.Status != EquipmentStatus.Reserved)
                throw new InvalidOperationException("Udstyret er ikke tilgængelig!");

            Reservation? reservation = null;
            if (item.Status == EquipmentStatus.Reserved)
            {
                reservation = await _db.Reservations
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.EquipmentId == item.Id && r.UserId == userId && r.Status == ReservationStatus.Active);

                if (reservation is null)
                    throw new InvalidOperationException("Du har ikke reserveret det her udstyret.");

                reservation.Status = ReservationStatus.Fulfilled;
            }

            var now = DateTime.Now;

            var loan = new Loan
            {
                UserId = userId,
                StartAt = DateTime.UtcNow,
                DueAt = dueAt,
                Status = LoanStatus.Active,
                Items = new List<LoanItem>() 
            };

            loan.Items.Add(new LoanItem
            {
                Loan = loan,
                Equipment = item
            });


            item.Status = EquipmentStatus.Loaned;
            _db.Loans.Add(loan);

            // ✅ EMAIL LOGIC
            var user = reservation?.User ?? await _db.Users.FindAsync(userId);
            var toEmail = user?.Email;
            var equipmentName = item.Name;
            var qrCodeText = item.QrCode;

            if (!string.IsNullOrWhiteSpace(toEmail))
            {
                var qr = new GenerateQRCode();
                using var qrStream = qr.GenerateQRCodeGen(qrCodeText, null);

                var body = $@"
            <h3>Loan Confirmed via Scan</h3>
            <p><b>Equipment:</b> {equipmentName}</p>
            <p><b>Start:</b> {now}</p>
            <p><b>Due:</b> {dueAt}</p>
            <p>Reference: LOAN-SCAN-{loan.Id}</p>";

                await _email.SendAsync(
                    toEmail,
                    $"Loan Created via Scan: {equipmentName}",
                    body,
                    qrStream,
                    "loan_qr.png"
                );
            }

            await _db.SaveChangesAsync();
            return loan;
        }



        //public async Task<Loan?> ReturnByScanAsync(int userId, string qrCode)
        //{
        //    var item = await _db.Equipment.SingleOrDefaultAsync(e => e.QrCode == qrCode);
        //    if (item is null) return null;

        //    // find active loan containing this item
        //    var loan = await _db.Loans
        //        .Include(l => l.Items)
        //        .Where(l => l.Status == LoanStatus.Active && l.Items.Any(i => i.EquipmentId == item.Id))
        //        .OrderByDescending(l => l.StartAt)
        //        .FirstOrDefaultAsync();

        //    if (loan is null) return null;

        //    loan.ReturnedAt = DateTime.UtcNow;
        //    loan.Status = LoanStatus.Closed;
        //    item.Status = EquipmentStatus.Available;

        //    await _db.SaveChangesAsync();
        //    return loan;
        //}
        public async Task<Loan?> ReturnByScanAsync(int userId, string qrCode)
        {
            var item = await _db.Equipment.SingleOrDefaultAsync(e => e.QrCode == qrCode);
            if (item is null) return null;

            var loan = await _db.Loans
                .Include(l => l.Items)
                    .ThenInclude(i => i.Equipment)
                .Include(l => l.User)
                .Where(l => l.Status == LoanStatus.Active && l.Items.Any(i => i.EquipmentId == item.Id))
                .OrderByDescending(l => l.StartAt)
                .FirstOrDefaultAsync();


            if (loan is null) return null;

            loan.ReturnedAt = DateTime.UtcNow;
            loan.Status = LoanStatus.Closed;
            item.Status = EquipmentStatus.Available;

            // ✅ EMAIL LOGIC with try/catch
            try
            {
                var toEmail = loan.User?.Email;
                var equipmentName = loan.Items.FirstOrDefault()?.Equipment?.Name ?? "Unknown";


                if (!string.IsNullOrWhiteSpace(toEmail))
                {
                    var body = $@"
                <h3>Udstyr Returneret</h3>
                <p><b>Udstyr:</b> {equipmentName}</p>
                <p><b>Returneret:</b> {loan.ReturnedAt}</p>
                <p>Tak for din aflevering.</p>";

                    await _email.SendAsync(
                        toEmail,
                        $"Return Confirmed: {equipmentName}",
                        body
                    );
                }
            }
            catch (Exception ex)
            {
                // log or debug here if needed
                Console.WriteLine($"[EMAIL ERROR ReturnByScanAsync]: {ex.Message}");
                // you can also log to a DB or logger instead of Console
            }

            await _db.SaveChangesAsync();
            return loan;
        }

    }


}
