using Loanity.API.Controllers.Common;
using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers.Crud
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : CrudControllerAPI<EquipmentCategory>
    {
        private static readonly HashSet<string> AllowedIcons = new(StringComparer.OrdinalIgnoreCase)
        {
            "box","laptop","tablet","keyboard","phone","mouse","printer","storage",
            "camera","headset","network","cloud","monitor","gamepad","cable","toolbox"
        };

        public CategoriesController(LoanityDbContext db) : base(db) { }

        // CREATE
        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] EquipmentCategory category)
        {
            category.Name = category.Name?.Trim() ?? "";
            category.Icon = string.IsNullOrWhiteSpace(category.Icon) ? "box" : category.Icon.Trim();

            if (string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Name is required.");
            if (category.Quantity < 0)
                return BadRequest("Quantity can't be negative.");
            if (!AllowedIcons.Contains(category.Icon))
                return BadRequest($"Icon '{category.Icon}' is not allowed.");

            // unique name guard
            var exists = await _db.EquipmentCategories.AnyAsync(x => x.Name == category.Name);
            if (exists) return Conflict($"Category '{category.Name}' already exists.");

            _db.EquipmentCategories.Add(category);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        // READ single (used by CreatedAtAction)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await _db.EquipmentCategories.FindAsync(id);
            return cat is null ? NotFound() : Ok(cat);
        }

        // UPDATE
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EquipmentCategory patch)
        {
            var cat = await _db.EquipmentCategories.FindAsync(id);
            if (cat is null) return NotFound();

            var newName = (patch.Name ?? "").Trim();
            var newIcon = string.IsNullOrWhiteSpace(patch.Icon) ? cat.Icon : patch.Icon.Trim();

            if (string.IsNullOrWhiteSpace(newName))
                return BadRequest("Name is required.");
            if (patch.Quantity < 0)
                return BadRequest("Quantity can't be negative.");
            if (!AllowedIcons.Contains(newIcon))
                return BadRequest($"Icon '{newIcon}' is not allowed.");

            // unique name guard (exclude self)
            var nameUsed = await _db.EquipmentCategories
                .AnyAsync(x => x.Id != id && x.Name == newName);
            if (nameUsed) return Conflict($"Category '{newName}' already exists.");

            cat.Name = newName;
            cat.Quantity = patch.Quantity;
            cat.Icon = newIcon;

            await _db.SaveChangesAsync();
            return Ok(cat);
        }
    }
}
