
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

        // Static dictionary to track login attempts per IP
        public static Dictionary<string, (int Count, DateTime LastAttempt)> LoginAttempts = new();

        private const int MAX_ATTEMPTS = 3;
        private static readonly TimeSpan COOLDOWN = TimeSpan.FromMinutes(1);

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            //  1. Validate login FIRST
            var user = _db.Users.FirstOrDefault(u =>
                u.UserName == dto.UserName &&
                u.PassWord == dto.PassWord
            );

            // If login is invalid
            if (user == null)
            {
                // Increment failed attempts
                if (LoginAttempts.TryGetValue(ip, out var attempt))
                {
                    var (count, lastAttempt) = attempt;

                    if (DateTime.UtcNow - lastAttempt < COOLDOWN)
                    {
                        LoginAttempts[ip] = (count + 1, DateTime.UtcNow);

                        if (count + 1 >= MAX_ATTEMPTS)
                        {
                            //return BadRequest("⏳ Too many login attempts. Try again in 1 minute.");
                            return BadRequest("3x attempts");
                        }
                        LoginAttempts[ip] = (count + 1, DateTime.UtcNow);
                    }

                    else
                    {
                        // Cooldown expired → reset count
                        //LoginAttempts[ip] = (1, DateTime.UtcNow);
                        LoginAttempts.Remove(ip);
                    }
                }
                else
                {
                    // First failed attempt
                    LoginAttempts[ip] = (1, DateTime.UtcNow);
                }

                return Unauthorized("❌ Invalid login credentials");
            }

            //  2. RFID check for Admins
            if (user.RoleId == 1)
            {
                if (string.IsNullOrWhiteSpace(dto.RfidChip) || dto.RfidChip != user.RfidChip)
                    return Unauthorized("🔒 RFID is required for Admin login.");
            }

            //  3. Success → clear rate limit record
            LoginAttempts.Remove(ip);

            //  4. Return user
            var result = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.UserName,
                user.RoleId,
                user.Email,
                user.RfidChip
            };

            return Ok(result);
        }


    }


}


