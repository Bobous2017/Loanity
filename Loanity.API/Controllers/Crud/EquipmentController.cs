//using Loanity.Domain.Dtos.CategoryType;
//using Loanity.Domain.Entities;
//using Loanity.Domain.IServices;
//using Loanity.Domain.Statuses;
//using Loanity.Infrastructure;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Loanity.API.Controllers
//{


//    [ApiController]
//    [Route("api/equipment")]
//    public class EquipmentController : ControllerBase
//    {
//        private readonly LoanityDbContext _db;
//        public EquipmentController(LoanityDbContext db) => _db = db;



//        // ----------------- READ -----------------
//        [HttpGet]
//        public async Task<IActionResult> Get() =>
//            Ok(await _db.Equipment.Include(e => e.Category).ToListAsync());

//        //----------------- READ by id-----------------
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(int id)
//        {
//            var equipment = await _db.Equipment.Include(e => e.Category)
//                                               .FirstOrDefaultAsync(e => e.Id == id);
//            return equipment == null ? NotFound() : Ok(equipment);
//        }

//        //----------------- READ by qr-----------------
//        [HttpGet("by-qr/{qr}")]
//        public async Task<IActionResult> GetByQr(string qr) =>
//            Ok(await _db.Equipment.SingleOrDefaultAsync(e => e.QrCode == qr));

//        // ----------------- CREATE -----------------
//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] EquipmentCreateDto dto)
//        {
//            var equipment = new Equipment
//            {
//                Name = dto.Name,
//                SerialNumber = dto.SerialNumber,
//                QrCode = dto.QrCode,
//                Status = dto.Status,
//                CategoryId = dto.CategoryId
//            };

//            _db.Equipment.Add(equipment);
//            await _db.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetById), new { id = equipment.Id }, equipment);
//        }

//        // ----------------- UPDATE -----------------
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, [FromBody] EquipmentUpdateDto dto)
//        {
//            if (id != dto.Id) return BadRequest();

//            var existing = await _db.Equipment.FindAsync(id);
//            if (existing == null) return NotFound();

//            existing.Name = dto.Name;
//            existing.SerialNumber = dto.SerialNumber;
//            existing.QrCode = dto.QrCode;
//            existing.Status = dto.Status;
//            existing.CategoryId = dto.CategoryId;

//            await _db.SaveChangesAsync();
//            return NoContent();
//        }


//        // ----------------- DELETE -----------------
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var existing = await _db.Equipment.FindAsync(id);
//            if (existing == null) return NotFound();

//            _db.Equipment.Remove(existing);
//            await _db.SaveChangesAsync();
//            return NoContent();
//        }
//    }


//}
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
    }
}
