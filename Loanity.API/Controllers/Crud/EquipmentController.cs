using Loanity.API.Controllers.Common;
using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers.Crud
{
    [ApiController]
    [Route("api/equipment")]
    public class EquipmentController : CrudControllerAPI<Equipment>
    {
        // Constructor
        public EquipmentController(LoanityDbContext db) : base(db) { }

        [HttpGet("{id:int}")]
        public override async Task<IActionResult> GetById(int id)
        {
            var equipment = await _db.Equipment
                                     .Include(e => e.Category)
                                     .FirstOrDefaultAsync(e => e.Id == id);
            return equipment == null ? NotFound() : Ok(equipment);
        }

        // ----------------- READ by multiple IDs -----------------
        [HttpGet("{ids}")]
        public async Task<IActionResult> GetByIds(string ids)
        {
            var idList = ids.Split(',')
                            .Select(idStr => int.TryParse(idStr, out var id) ? id : (int?)null)
                            .Where(id => id.HasValue)
                            .Select(id => id.Value)
                            .ToList();

            if (idList.Count == 0)
                return BadRequest("No valid IDs provided.");

            var equipmentList = await _db.Equipment
                                         .Include(e => e.Category)
                                         .Where(e => idList.Contains(e.Id))
                                         .ToListAsync();

            return Ok(equipmentList);
        }

        [HttpGet("by-qr/{qr}")]
        public async Task<IActionResult> GetByQr(string qr)
        {
            var item = await _db.Equipment.SingleOrDefaultAsync(e => e.QrCode == qr);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var equipmentList = await _db.Equipment
                                         .Include(e => e.Category)
                                         .ToListAsync();
            return Ok(equipmentList);
        }
    }
}
