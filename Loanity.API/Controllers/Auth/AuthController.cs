
    using global::Loanity.Domain.Dtos;
    using global::Loanity.Infrastructure;
    using Loanity.Domain.Dtos;
    using Loanity.Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    namespace Loanity.API.Controllers.Auth
    {
        [ApiController]
        [Route("api/auth")]
        public class AuthController : ControllerBase
        {
            private readonly LoanityDbContext _db;

            public AuthController(LoanityDbContext db) => _db = db;

            [HttpPost("login")]
            public IActionResult Login([FromBody] LoginDto dto)
            {
                var userQuery = _db.Users.Where(u =>
                    u.Username == dto.Username &&
                    u.Password == dto.Password
                );

                if (!string.IsNullOrEmpty(dto.RfidChip))
                    userQuery = userQuery.Where(u => u.RfidChip == dto.RfidChip);

                var user = userQuery.Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Username,
                    u.Password,
                    u.RoleId,
                    u.Email,
                    u.RfidChip
                }).FirstOrDefault();

                if (user == null)
                    return Unauthorized("Invalid login credentials");

                return Ok(user);
            }
        }
    }


