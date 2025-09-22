using Loanity.API.Controllers.Common;
using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanity.API.Controllers.Crud
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : CrudControllerAPI<Role>
    {
        public RoleController(LoanityDbContext db) : base(db) { }

        // ----------------- CREATE -----------------
        //logic that checks if the role name already exists during create
        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] Role role)
        {
            var exists = await _db.Roles.AnyAsync(r => r.Name == role.Name);
            if (exists)
                return BadRequest("Role already exists.");

            return await base.Create(role);
        }

    }
}
