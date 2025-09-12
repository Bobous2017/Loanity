using Loanity.Domain.Entities;
using Loanity.Domain.IServices;
using Loanity.Domain.Statuses;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers
{


    [ApiController]
    [Route("api/equipment")]
    public class EquipmentController : ControllerBase
    {
        private readonly LoanityDbContext _db;
        public EquipmentController(LoanityDbContext db) => _db = db;

        // ----------------- READ -----------------
        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _db.Equipment.Include(e => e.Category).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var equipment = await _db.Equipment.Include(e => e.Category)
                                               .FirstOrDefaultAsync(e => e.Id == id);
            return equipment == null ? NotFound() : Ok(equipment);
        }

        [HttpGet("by-qr/{qr}")]
        public async Task<IActionResult> GetByQr(string qr) =>
            Ok(await _db.Equipment.SingleOrDefaultAsync(e => e.QrCode == qr));

        // ----------------- CREATE -----------------
        public record EquipmentCreateDto( //   Record EquipmentCreateDto
         string Name,
         string SerialNumber,
         string QrCode,
         EquipmentStatus Status,
         int CategoryId);

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EquipmentCreateDto dto)
        {
            var equipment = new Equipment
            {
                Name = dto.Name,
                SerialNumber = dto.SerialNumber,
                QrCode = dto.QrCode,
                Status = dto.Status,
                CategoryId = dto.CategoryId
            };

            _db.Equipment.Add(equipment);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = equipment.Id }, equipment);
        }

        // ----------------- UPDATE -----------------
        public record EquipmentUpdateDto( //   Record EquipmentUpdateDto
            int Id,
            string Name,
            string SerialNumber,
            string QrCode,
            EquipmentStatus Status,
            int CategoryId
        );

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EquipmentUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var existing = await _db.Equipment.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = dto.Name;
            existing.SerialNumber = dto.SerialNumber;
            existing.QrCode = dto.QrCode;
            existing.Status = dto.Status;
            existing.CategoryId = dto.CategoryId;

            await _db.SaveChangesAsync();
            return NoContent();
        }


        // ----------------- DELETE -----------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.Equipment.FindAsync(id);
            if (existing == null) return NotFound();

            _db.Equipment.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

   
}
