using Loanity.API.Controllers.Common;
using Loanity.Domain.Entities;
using Loanity.Domain.Statuses;
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

        // ----------------- READ by Category Name -----------------
        [HttpGet("category/{name}")]
        public async Task<IActionResult> GetByCategory(string name)
        {
            var equipment = await _db.Equipment
                .Include(e => e.Category)
                .Where(e => e.Category.Name == name)
                .ToListAsync();

            return Ok(equipment);
        }

        // ----------------- Category Summary -----------------
        [HttpGet("category-summary/{name}")]
        public async Task<IActionResult> GetSummary(string name)
        {
            var items = await _db.Equipment
                                 .Include(e => e.Category)
                                 .Where(e => e.Category.Name == name)
                                 .ToListAsync();

            var result = new
            {
                Total = items.Count,
                Available = items.Count(e => e.Status == EquipmentStatus.Available),
                Reserved = items.Count(e => e.Status == EquipmentStatus.Reserved),
                Loaned = items.Count(e => e.Status == EquipmentStatus.Loaned),
                Damaged = items.Count(e => e.Status == EquipmentStatus.Damaged),
                Lost = items.Count(e => e.Status == EquipmentStatus.Lost),
                Equipment = items
            };

            return Ok(result);
        }

        // ----------------- All Categories With Quantity -----------------
        [HttpGet("categories-with-quantity")]
        public async Task<IActionResult> GetCategoriesWithQuantity()
        {
            var data = await _db.EquipmentCategories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    Quantity = _db.Equipment.Count(e => e.CategoryId == c.Id)
                })
                .ToListAsync();

            return Ok(data);
        }




    }
}
