using Loanity.Domain.Dtos;
using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly LoanityDbContext _context;

        public LoanController(LoanityDbContext context)
        {
            _context = context;
        }

        // GET: api/loan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans()
        {
            var loans = await _context.Loans
                .Include(l => l.Items)
                    .ThenInclude(i => i.Equipment)
                .ToListAsync();

            var dtoList = loans.Select(loan => new LoanDto(
                loan.Id,
                loan.UserId,
                loan.StartAt,
                loan.DueAt,
                loan.ReturnedAt,
                loan.Status.ToString(), //  enum -> string
                loan.ReservationId,
                loan.Items.Select(item => new LoanItemDto(
                    item.EquipmentId,
                    item.Equipment.Name
                )).ToList()
            )).ToList();


            return Ok(dtoList);
        }

        // GET: api/loan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanDto>> GetLoan(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Items)
                    .ThenInclude(i => i.Equipment)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return NotFound();

            var dto = new LoanDto(
            loan.Id,
            loan.UserId,
            loan.StartAt,
            loan.DueAt,
            loan.ReturnedAt,
            loan.Status.ToString(),
            loan.ReservationId,
            loan.Items.Select(item => new LoanItemDto(
                item.EquipmentId,
                item.Equipment.Name
            )).ToList()
        );

            return Ok(dto);

        }
    }
}
