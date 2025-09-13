//namespace Loanity.API.Controllers
//{
//    using global::Loanity.Domain.Entities;
//    using global::Loanity.Infrastructure;
//    using Microsoft.AspNetCore.Mvc;
//    using Microsoft.EntityFrameworkCore;

//    namespace Loanity.API.Controllers
//    {
//        [ApiController]
//        [Route("api/[controller]")]
//        public class ReservationController : ControllerBase
//        {
//            private readonly LoanityDbContext _db;

//            public ReservationController(LoanityDbContext db)
//            {
//                _db = db;
//            }

//            [HttpGet]
//            public async Task<IActionResult> GetAll()
//            {
//                var reservations = await _db.Reservations.ToListAsync();
//                return Ok(reservations);
//            }

//            [HttpGet("{id}")]
//            public async Task<IActionResult> GetById(int id)
//            {
//                var reservation = await _db.Reservations.FindAsync(id);
//                if (reservation == null) return NotFound();
//                return Ok(reservation);
//            }

//            [HttpPost]
//            public async Task<IActionResult> Create([FromBody] Reservation reservation)
//            {
//                _db.Reservations.Add(reservation);
//                await _db.SaveChangesAsync();
//                return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
//            }

//            [HttpPut("{id}")]
//            public async Task<IActionResult> Update(int id, [FromBody] Reservation updated)
//            {
//                if (id != updated.Id) return BadRequest();

//                var existing = await _db.Reservations.FindAsync(id);
//                if (existing == null) return NotFound();

//                // update fields manually
//                existing.StartAt = updated.StartAt;
//                existing.EndAt = updated.EndAt;
//                existing.UserId = updated.UserId;
//                existing.EquipmentId = updated.EquipmentId;
//                existing.Status = updated.Status;

//                _db.Entry(existing).State = EntityState.Modified;
//                await _db.SaveChangesAsync();
//                return NoContent();
//            }

//            [HttpDelete("{id}")]
//            public async Task<IActionResult> Delete(int id)
//            {
//                var existing = await _db.Reservations.FindAsync(id);
//                if (existing == null) return NotFound();

//                _db.Reservations.Remove(existing);
//                await _db.SaveChangesAsync();
//                return NoContent();
//            }
//        }
//    }

//}
using Loanity.Domain.Entities;
using Loanity.Domain.IServices;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly LoanityDbContext _db;

        public ReservationController(IReservationService reservationService, LoanityDbContext db)
        {
            _reservationService = reservationService;
            _db = db;
        }

        //  GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reservations = await _db.Reservations.ToListAsync();
            return Ok(reservations);
        }
      

        //  GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reservation = await _db.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        //  CREATE (Business logic)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Reservation reservation)
        {
            try
            {
                var created = await _reservationService.CreateAsync(
                    reservation.UserId,
                    reservation.EquipmentId,
                    reservation.StartAt,
                    reservation.EndAt
                );

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //  UPDATE (Business logic)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Reservation updated)
        {
            if (id != updated.Id) return BadRequest();

            try
            {
                var success = await _reservationService.UpdateAsync(updated);
                if (!success) return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //  DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.Reservations.FindAsync(id);
            if (existing == null) return NotFound();

            _db.Reservations.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        //  FULFILL
        [HttpPost("{id}/fulfill")]
        public async Task<IActionResult> Fulfill(int id)
        {
            var success = await _reservationService.FulfillReservationAsync(id);
            if (!success) return NotFound(new { message = "Reservation not found or already fulfilled" });
            return Ok(new { message = "Reservation fulfilled successfully" });
        }

        //  CANCEL
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _reservationService.CancelReservationAsync(id);
            if (!success) return NotFound(new { message = "Reservation not found or already cancelled" });
            return Ok(new { message = "Reservation cancelled successfully" });
        }

        //  EXPIRE
        [HttpPost("{id}/expire")]
        public async Task<IActionResult> Expire(int id)
        {
            var success = await _reservationService.ExpireReservationAsync(id);
            if (!success) return NotFound(new { message = "Reservation not found or already expired" });
            return Ok(new { message = "Reservation expired successfully" });
        }
    }
}
