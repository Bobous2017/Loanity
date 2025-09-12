using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    //private static readonly List<Role> Roles = new()
    //{
    //    new Role { Id = 1, Name = "Admin" },
    //    new Role { Id = 2, Name = "User" }
    //};

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetAll()
    {
        var roles = await _db.Roles.ToListAsync();
        return Ok(roles);
    }


    private readonly LoanityDbContext _db;
    public RoleController(LoanityDbContext db) => _db = db;

    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<Role>>> GetAll() =>
    //    Ok(await _db.Roles.ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Role>> GetById(int id)
    {
        var role = await _db.Roles.FindAsync(id);
        return role == null ? NotFound() : Ok(role);
    }

   
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Role role)
    {
        var exists = await _db.Roles.AnyAsync(r => r.Name == role.Name);
        if (exists)

            return BadRequest("Role already exists.");
        _db.Roles.Add(role);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Role updated)
    {
        if (id != updated.Id) return BadRequest();

        var existing = await _db.Roles.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = updated.Name; 

        await _db.SaveChangesAsync(); 

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var role = await _db.Roles.FindAsync(id);
        if (role == null) return NotFound();

        _db.Roles.Remove(role);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
