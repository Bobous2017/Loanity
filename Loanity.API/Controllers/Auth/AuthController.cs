
    using global::Loanity.Domain.Dtos;
    using global::Loanity.Infrastructure;
    using Loanity.Domain.Dtos;
    using Loanity.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

    namespace Loanity.API.Controllers.Auth
    {
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly LoanityDbContext _db;
        private readonly IConfiguration _config;
        public AuthController(LoanityDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // Static dictionary to track login attempts per IP
        public static Dictionary<string, (int Count, DateTime LastAttempt)> LoginAttempts = new();

        private const int MAX_ATTEMPTS = 3;
        private static readonly TimeSpan COOLDOWN = TimeSpan.FromMinutes(1);


        [Authorize]
        [HttpGet]
        public IActionResult GetSecureData()
        {
            return Ok("Only authorized users see this");
        }


        //[HttpPost("login")]
        //public IActionResult Login([FromBody] LoginDto dto)
        //{
        //    var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        //    //  1. Validate login FIRST
        //    var user = _db.Users.FirstOrDefault(u =>
        //        u.UserName == dto.UserName &&
        //        u.PassWord == dto.PassWord
        //    );

        //    // If login is invalid
        //    if (user == null)
        //    {
        //        // Increment failed attempts
        //        if (LoginAttempts.TryGetValue(ip, out var attempt))
        //        {
        //            var (count, lastAttempt) = attempt;

        //            if (DateTime.UtcNow - lastAttempt < COOLDOWN)
        //            {
        //                LoginAttempts[ip] = (count + 1, DateTime.UtcNow);

        //                if (count + 1 >= MAX_ATTEMPTS)
        //                {
        //                    //return BadRequest("⏳ Too many login attempts. Try again in 1 minute.");
        //                    return BadRequest("3x attempts");
        //                }
        //                LoginAttempts[ip] = (count + 1, DateTime.UtcNow);
        //            }

        //            else
        //            {
        //                // Cooldown expired → reset count
        //                //LoginAttempts[ip] = (1, DateTime.UtcNow);
        //                LoginAttempts.Remove(ip);
        //            }
        //        }
        //        else
        //        {
        //            // First failed attempt
        //            LoginAttempts[ip] = (1, DateTime.UtcNow);
        //        }

        //        return Unauthorized("❌ Invalid login credentials");
        //    }

        //    //  2. RFID check for Admins
        //    if (user.RoleId == 1)
        //    {
        //        if (string.IsNullOrWhiteSpace(dto.RfidChip) || dto.RfidChip != user.RfidChip)
        //            return Unauthorized("🔒 RFID is required for Admin login.");
        //    }

        //    //  3. Success → clear rate limit record
        //    LoginAttempts.Remove(ip);

        //    //  4. Return user
        //    var result = new
        //    {
        //        user.Id,
        //        user.FirstName,
        //        user.LastName,
        //        user.UserName,
        //        user.RoleId,
        //        user.Email,
        //        user.RfidChip
        //    };

        //    return Ok(result);
        //}



        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _db.Users.FirstOrDefault(u =>
                u.UserName == dto.UserName &&
                u.PassWord == dto.PassWord);

            if (user == null)
                return Unauthorized("Invalid credentials");

            // RFID check for Admins
            if (user.RoleId == 1 && (string.IsNullOrWhiteSpace(dto.RfidChip) || dto.RfidChip != user.RfidChip))
                return Unauthorized("🔒 RFID is required for Admin login.");

            // 1. ✅ Create Claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Admin" : "User")
    };

            // 2. ✅ Generate JWT token
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"])),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            // 3. ✅ Return token (for Postman, mobile, etc.)
            return Ok(new
            {
                token = jwt,
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.RoleId,
                    user.FirstName,
                    user.LastName
                }
            });
        }

    }


}


