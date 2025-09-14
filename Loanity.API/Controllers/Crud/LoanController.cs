//using Loanity.Domain.Dtos;
//using Loanity.Domain.Entities;
//using Loanity.Infrastructure;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Loanity.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class LoanController : ControllerBase
//    {
//        private readonly LoanityDbContext _context;

//        public LoanController(LoanityDbContext context)
//        {
//            _context = context;
//        }

//        // ----------------- READ All-----------------
//        // GET: api/loan
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans()
//        {
//            var loans = await _context.Loans
//                .Include(l => l.Items)
//                    .ThenInclude(i => i.Equipment)
//                .ToListAsync();

//            var dtoList = loans.Select(loan => new LoanDto(
//                loan.Id,
//                loan.UserId,
//                loan.StartAt,
//                loan.DueAt,
//                loan.ReturnedAt,
//                loan.Status.ToString(), //  enum -> string
//                loan.ReservationId,
//                loan.Items.Select(item => new LoanItemDto(
//                    item.EquipmentId,
//                    item.Equipment.Name
//                )).ToList()
//            )).ToList();


//            return Ok(dtoList);
//        }

//        // ----------------- READ by ID-----------------
//        // GET: api/loan/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<LoanDto>> GetLoan(int id)
//        {
//            var loan = await _context.Loans
//                .Include(l => l.Items)
//                    .ThenInclude(i => i.Equipment)
//                .FirstOrDefaultAsync(l => l.Id == id);

//            if (loan == null)
//                return NotFound();

//            var dto = new LoanDto(
//            loan.Id,
//            loan.UserId,
//            loan.StartAt,
//            loan.DueAt,
//            loan.ReturnedAt,
//            loan.Status.ToString(),
//            loan.ReservationId,
//            loan.Items.Select(item => new LoanItemDto(
//                item.EquipmentId,
//                item.Equipment.Name
//            )).ToList()
//        );

//            return Ok(dto);

//        }
//    }
//}
using Loanity.API.Controllers.Common;
using Loanity.Domain.Dtos;
using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers.Crud
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanController : CrudControllerAPI<Loan>
    {
        public LoanController(LoanityDbContext context) : base(context) { }

        // ----------------- READ All with DTO -----------------
        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var loans = await _db.Loans
                .Include(l => l.Items)
                    .ThenInclude(i => i.Equipment)
                .ToListAsync();

            var dtoList = loans.Select(loan => new LoanDto(
                loan.Id,
                loan.UserId,
                loan.StartAt,
                loan.DueAt,
                loan.ReturnedAt,
                loan.Status.ToString(), // enum to string
                loan.ReservationId,
                loan.Items.Select(item => new LoanItemDto(
                    item.EquipmentId,
                    item.Equipment.Name
                )).ToList()
            )).ToList();

            return Ok(dtoList);
        }

        // ----------------- READ by ID with DTO -----------------
        [HttpGet("{id}")]
        public override async Task<IActionResult> GetById(int id)
        {
            var loan = await _db.Loans
                .Include(l => l.Items)
                    .ThenInclude(i => i.Equipment)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null) return NotFound();

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
