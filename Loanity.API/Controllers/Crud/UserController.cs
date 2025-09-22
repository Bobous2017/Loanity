using Loanity.API.Controllers.Common;
using Loanity.Domain.Dtos;
using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Loanity.API.Controllers.Crud
{
    [ApiController]
    [Route("api/user")]
    public class UsersController : CrudControllerAPI<User>
    {
        public UsersController(LoanityDbContext db) : base(db) { }

        // Override GET ALL to return UserDto with Role.Name
        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var users = await _db.Users.Include(u => u.Role).ToListAsync();

            var userDtos = users.Select(u => new UserDto(
                u.Id,
                u.FirstName,
                u.LastName,
                u.UserName,
                u.PassWord,
                u.RfidChip,
                u.Email,
                u.Phone,
                u.RoleId,
                u.Role?.Name
            )).ToList();

            return Ok(userDtos);
        }
        // Creating User  med Hashe  Password
        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] User user)
        {
            if (!string.IsNullOrWhiteSpace(user.PassWord))
            {
                using var sha256 = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(user.PassWord);
                var hash = sha256.ComputeHash(bytes);

                //  Convert to HEX string
                var sb = new StringBuilder();
                foreach (var b in hash)
                    sb.Append(b.ToString("x2"));

                user.PassWord = sb.ToString();
            }

            return await base.Create(user);
        }

        // Optional: keep default GetById if DTO isn't needed
        // Otherwise, override it the same way as above.  Now

    }
}
