
//using Loanity.Domain.Entities;
//using Loanity.Domain.IServices;
//using Loanity.Infrastructure;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Loanity.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class ReservationController : ControllerBase
//    {
//        private readonly LoanityDbContext _db;

//        public ReservationController(LoanityDbContext db)
//        {
//            _db = db;
//        }

//        // ----------------- READ All-----------------
//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var reservations = await _db.Reservations.ToListAsync();
//            return Ok(reservations);
//        }

//        // ----------------- READ by ID-----------------
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(int id)
//        {
//            var reservation = await _db.Reservations.FindAsync(id);
//            return reservation == null ? NotFound() : Ok(reservation);
//        }

//        // ----------------- CREATE -----------------
//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] Reservation reservation)
//        {
//            _db.Reservations.Add(reservation);
//            await _db.SaveChangesAsync();
//            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
//        }

//        // ----------------- UPDATE -----------------
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, [FromBody] Reservation updated)
//        {
//            if (id != updated.Id) return BadRequest();

//            var existing = await _db.Reservations.FindAsync(id);
//            if (existing == null) return NotFound();

//            existing.StartAt = updated.StartAt;
//            existing.EndAt = updated.EndAt;
//            existing.UserId = updated.UserId;
//            existing.EquipmentId = updated.EquipmentId;
//            existing.Status = updated.Status;

//            await _db.SaveChangesAsync();
//            return NoContent();
//        }

//        //  ----------------- DELETE -----------------
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var existing = await _db.Reservations.FindAsync(id);
//            if (existing == null) return NotFound();

//            _db.Reservations.Remove(existing);
//            await _db.SaveChangesAsync();
//            return NoContent();
//        }
//    }

//}

using Loanity.API.Controllers.Common;
using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.API.Controllers.Crud
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : CrudControllerAPI<Reservation>
    {
        public ReservationController(LoanityDbContext db) : base(db) { }
    }
}
