using Loanity.Domain.Entities;
using Loanity.Domain.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.API.Controllers.Actions
{
    [ApiController]
    [Route("api/reservation-action")]
    public class ReservationActionController : ControllerBase
    {
        private readonly IReservationService _service;

        public ReservationActionController(IReservationService service)
        {
            _service = service;
        }

        // POST: Create (Business logic)
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Reservation reservation)
        {
            try
            {
                var created = await _service.CreateAsync(
                    reservation.UserId,
                    reservation.EquipmentId,
                    reservation.StartAt,
                    reservation.EndAt
                );

                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // PUT: Update (Business logic) from ReservationActionController
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] Reservation updated)
        {
            try
            {
                var success = await _service.UpdateAsync(updated);
                if (!success) return NotFound();
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: Fulfill
        [HttpPost("{id}/fulfill")]
        public async Task<IActionResult> Fulfill(int id)
        {
            var success = await _service.FulfillReservationAsync(id);
            if (!success) return NotFound(new { message = "Reservation not found or already fulfilled" });
            return Ok(new { message = "Reservation fulfilled" });
        }

        // POST: Cancel
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _service.CancelReservationAsync(id);
            if (!success) return NotFound(new { message = "Reservation not found or already canceled" });
            return Ok(new { message = "Reservation canceled" });
        }

        // POST: Expire
        [HttpPost("{id}/expire")]
        public async Task<IActionResult> Expire(int id)
        {
            var success = await _service.ExpireReservationAsync(id);
            if (!success) return NotFound(new { message = "Reservation not found or already expired" });
            return Ok(new { message = "Reservation expired" });
        }
    }
}
