
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


       
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            // Get client IP (you can customize this if behind reverse proxy)
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Check rate limit
            if (LoginAttempts.TryGetValue(ip, out var info))
            {
                if (info.Count >= MAX_ATTEMPTS && DateTime.UtcNow - info.LastAttempt < COOLDOWN)
                {
                    return Unauthorized("3x attempts"); // Besked  key til Razor at vise countdown
                }
            }

            // Authenticate user
            var user = _db.Users.FirstOrDefault(u =>
                u.UserName == dto.UserName &&
                u.PassWord == dto.PassWord);

            if (user == null)
            {
                // Record failed attempt
                if (!LoginAttempts.ContainsKey(ip))
                    LoginAttempts[ip] = (1, DateTime.UtcNow);
                else
                    LoginAttempts[ip] = (LoginAttempts[ip].Count + 1, DateTime.UtcNow);

                return Unauthorized("Invalid credentials");
            }

            // Check RFID for Admins
            if (user.RoleId == 1 && (string.IsNullOrWhiteSpace(dto.RfidChip) || dto.RfidChip != user.RfidChip))
            {
                return Unauthorized("🔒 RFID is required for Admin login.");
            }

            // Successful login: clear failed attempts
            if (LoginAttempts.ContainsKey(ip))
                LoginAttempts.Remove(ip);

            // Create Claims
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Admin" : "User")
                };

            // Create Token
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

            // Return result
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


