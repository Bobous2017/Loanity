using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly LoanityDbContext _db;
        public CategoriesController(LoanityDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _db.EquipmentCategories.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _db.EquipmentCategories.FindAsync(id);
            return category == null ? NotFound() : Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EquipmentCategory category)
        {
            _db.EquipmentCategories.Add(category);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EquipmentCategory updated)
        {
            if (id != updated.Id) return BadRequest();

            var existing = await _db.EquipmentCategories.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = updated.Name;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.EquipmentCategories.FindAsync(id);
            if (existing == null) return NotFound();

            _db.EquipmentCategories.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

}
