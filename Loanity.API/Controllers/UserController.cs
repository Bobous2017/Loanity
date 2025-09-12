using Loanity.Domain.Dtos;
using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers
{
    [ApiController]
    [Route("api/user")]

    public class UsersController : ControllerBase
    {
        private readonly LoanityDbContext _db;

        public UsersController(LoanityDbContext db) => _db = db;

        //[HttpGet]
        //public async Task<IActionResult> GetAll() =>
        //    Ok(await _db.Users.ToListAsync());

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _db.Users.Include(u => u.Role).ToListAsync();

            var userDtos = users.Select(u => new UserDto(
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Phone,
                u.RoleId,
                u.Role?.Name 
            )).ToList();

            return Ok(userDtos);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _db.Users.FindAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User updated)
        {
            if (id != updated.Id) return BadRequest();

            var existing = await _db.Users.FindAsync(id);
            if (existing == null) return NotFound();

            existing.FirstName = updated.FirstName;
            existing.LastName = updated.LastName;
            existing.Email = updated.Email;
            existing.Phone = updated.Phone;
            existing.RoleId = updated.RoleId;

            await _db.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
