using Loanity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private static readonly List<Role> Roles = new()
    {
        new Role { Id = 1, Name = "Admin" },
        new Role { Id = 2, Name = "User" }
    };

    [HttpGet]
    public ActionResult<IEnumerable<Role>> GetAll() => Ok(Roles);
}
