
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
                u.UserName == dto.UserName &&
                u.PassWord == dto.PassWord
            );

            // Load the user first (to check their role)
            var user = userQuery.FirstOrDefault();

            if (user == null)
                return Unauthorized("Invalid login credentials");

            // Enforce RFID check for Admins
            if (user.RoleId == 1) // Admin
            {
                if (string.IsNullOrEmpty(dto.RfidChip) || dto.RfidChip != user.RfidChip)
                {
                    return Unauthorized("RFID is required for Admin login.");
                }
            }

            // Return user DTO
            var result = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.UserName,
                user.PassWord,
                user.RoleId,
                user.Email,
                user.RfidChip
            };

            return Ok(result);
        }


    }
}


