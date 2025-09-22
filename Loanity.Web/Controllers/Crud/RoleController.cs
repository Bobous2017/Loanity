
using Loanity.Domain.Entities;
using Loanity.Web.Controllers.Common;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.Web.Controllers
{
    public class RoleController : CrudControllerWeb<Role>
    {
        public RoleController(IHttpClientFactory factory)
            : base(factory, "role") // maps to "api/role"
        {
        }
        //Show a custom message when create fails
        [HttpPost]
        public override async Task<IActionResult> Create(Role role)
        {
            var result = await base.Create(role);
            if (!ModelState.IsValid)
                ViewBag.Message = "Creation failed";
            return result;
        }

    }
}
