using Loanity.Domain.Entities;
using Loanity.Domain.IExports;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers.Export
{
    [ApiController]
    [Route("api/export")]
    public class ExportController : ControllerBase
    {
        private readonly LoanityDbContext _db;
        private readonly IExportService<Reservation> _reservationExport;
        private readonly IExportService<Loan> _loanExport;
        private readonly IExportService<User> _userExport;

        public ExportController(
            LoanityDbContext db,
            IExportService<Reservation> reservationExport,
            IExportService<Loan> loanExport,
            IExportService<User> userExport)
        {
            _db = db;
            _reservationExport = reservationExport;
            _loanExport = loanExport;
            _userExport = userExport;
        }

        // Endpoints for CSV exports
        [HttpGet("reservations/csv")]
        public IActionResult ExportReservationsCsv()
        {
            var data = _db.Reservations.ToList();
            var csvBytes = _reservationExport.ExportToCsv(data);
            return File(csvBytes, "text/csv", "reservations.csv");
        }

        [HttpGet("loans/csv")]
        public IActionResult ExportLoansCsv()
        {
            var loans = _db.Loans.ToList();
            var csv = _loanExport.ExportToCsv(loans);
            return File(csv, "text/csv", "loans.csv");
        }

        [HttpGet("users/csv")]
        public IActionResult ExportUsersCsv()
        {
            var users = _db.Users.ToList();
            var csv = _userExport.ExportToCsv(users);
            return File(csv, "text/csv", "users.csv");
        }


        // Endpoints for PDF exports
        [HttpGet("reservations/pdf")]
        public IActionResult ExportReservationsPdf()
        {
            try
            {
                var data = _db.Reservations.ToList();
                var pdf = _reservationExport.ExportToPdf(data);
                return File(pdf, "application/pdf", "reservations.pdf");
            }
            catch (Exception ex)
            {
                // Log to console or return details temporarily
                return StatusCode(500, $"PDF Export Error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        [HttpGet("loans/pdf")]
        public IActionResult ExportLoansPdf()
        {
            try
            {
                var data = _db.Loans.ToList();
                var pdf = _loanExport.ExportToPdf(data);
                return File(pdf, "application/pdf", "loans.pdf");
            }
            catch (Exception ex)
            {
                // Log to console or return details temporarily
                return StatusCode(500, $"PDF Export Error: {ex.Message}\n{ex.StackTrace}");
            }
           
        }

        [HttpGet("users/pdf")]
        public IActionResult ExportUsersPdf()
        {
            try
            {
                var data = _db.Users.Include(u => u.Role).ToList();
                var pdf = _userExport.ExportToPdf(data);
                return File(pdf, "application/pdf", "users.pdf");
            }
            catch (Exception ex)
            {
                // Log to console or return details temporarily
                return StatusCode(500, $"PDF Export Error: {ex.Message}\n{ex.StackTrace}");
            }
            
        }

    }

}
