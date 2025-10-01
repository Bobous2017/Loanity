using Loanity.Domain.Dtos.UserHandlingDto;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers.Statistics
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly LoanityDbContext _db;

        public StatisticsController(LoanityDbContext db) => _db = db;

        // 1. Most loaned equipment
        [HttpGet("most-loaned")]
        public async Task<IActionResult> GetMostLoanedEquipment()
        {
            var allLoans = await _db.Loans
                .Include(l => l.Items)
                .ThenInclude(i => i.Equipment)
                .Include(l => l.User)
                .ToListAsync();

            var dtos = allLoans.SelectMany(l => l.Items.Select(i => new UserLoanDto
            {
                LoanId = l.Id,
                UserFullName = $"{l.User.FirstName} {l.User.LastName}",
                UserEmail = l.User.Email,
                EquipmentName = i.Equipment.Name,
                StartAt = l.StartAt,
                DueAt = l.DueAt,
                ReturnedAt = l.ReturnedAt,
                Status = l.Status.ToString()
            })).ToList();

            var result = dtos
                .GroupBy(dto => dto.EquipmentName)
                .Select(g => new {
                    Equipment = g.Key,
                    TimesLoaned = g.Count()
                })
                .OrderByDescending(x => x.TimesLoaned)
                .Take(10)
                .ToList();

            return Ok(result);
        }

        // 2. Average loan duration
        [HttpGet("average-loan-duration")]
        public async Task<IActionResult> GetAverageLoanDuration()
        {
            var allLoans = await _db.Loans
                .Include(l => l.Items)
                .ThenInclude(i => i.Equipment)
                .Include(l => l.User)
                .ToListAsync();

            var dtos = allLoans.SelectMany(l => l.Items.Select(i => new UserLoanDto
            {
                LoanId = l.Id,
                UserFullName = $"{l.User.FirstName} {l.User.LastName}",
                UserEmail = l.User.Email,
                EquipmentName = i.Equipment.Name,
                StartAt = l.StartAt,
                DueAt = l.DueAt,
                ReturnedAt = l.ReturnedAt,
                Status = l.Status.ToString()
            })).ToList();

            var durations = dtos.Select(d => (d.DueAt - d.StartAt).TotalHours);
            var avgDuration = durations.Any() ? durations.Average() : 0;

            return Ok(new { AverageLoanDurationHours = Math.Round(avgDuration, 2) });
        }

        // 3. Users with most delays
        [HttpGet("most-delays")]
        public async Task<IActionResult> GetUsersWithMostDelays()
        {
            var allLoans = await _db.Loans
                .Include(l => l.Items)
                .ThenInclude(i => i.Equipment)
                .Include(l => l.User)
                .ToListAsync();

            var dtos = allLoans.SelectMany(l => l.Items.Select(i => new UserLoanDto
            {
                LoanId = l.Id,
                UserFullName = $"{l.User.FirstName} {l.User.LastName}",
                UserEmail = l.User.Email,
                EquipmentName = i.Equipment.Name,
                StartAt = l.StartAt,
                DueAt = l.DueAt,
                ReturnedAt = l.ReturnedAt,
                Status = l.Status.ToString()
            })).ToList();

            var result = dtos
                .Where(d => d.ReturnedAt.HasValue && d.ReturnedAt.Value > d.DueAt)
                .GroupBy(d => d.UserFullName)
                .Select(g => new {
                    User = g.Key,
                    DelayCount = g.Count()
                })
                .OrderByDescending(x => x.DelayCount)
                .Take(10)
                .ToList();

            return Ok(result);
        }
    }

}
