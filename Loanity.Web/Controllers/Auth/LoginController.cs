using Loanity.Domain.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Loanity.Web.Controllers.Auth
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _http;

        public LoginController(IHttpClientFactory http)
        {
            _http = http;
        }
       
        string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            //return Convert.ToBase64String(hash);  // admin:   --AND u.Password = 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg='
            //Convert to HEX string, to match DB format
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString(); // admin '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918' 
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginDto());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            //  Hash password before sending to API
            model.PassWord = HashPassword(model.PassWord);

            var client = _http.CreateClient("LoanityApi");

            var response = await client.PostAsJsonAsync("api/auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();

                // Create claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
            new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Admin" : "User")
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }

            //ModelState.AddModelError("", "Invalid login attempt"); // Feeedback to user

            // ❗ Read error message from API
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", errorMessage); // Show exact reason
            return View("Index", model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

    }
}
