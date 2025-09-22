using Loanity.API.Controllers.Common;
using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.API.Controllers.Crud
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : CrudControllerAPI<EquipmentCategory>
    {
        public CategoriesController(LoanityDbContext db) : base(db) { }

        // ------------------ CREATE with validation -----------------
        // Override Create to add validation or filtering
        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] EquipmentCategory category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Name is required.");

            return await base.Create(category);
        }

    }
}
