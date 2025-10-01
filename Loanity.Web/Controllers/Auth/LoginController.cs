using Loanity.Domain.AuthHelper;
using Loanity.Domain.Dtos;
using Loanity.Domain.Entities;
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
       
    

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginDto());
        }

        

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            // Manual validation
            if (string.IsNullOrWhiteSpace(model.RfidChip) &&
                (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.PassWord)))
            {
                ModelState.AddModelError("", "Udfyld enten RFID eller brugernavn og adgangskode.");
                return View("Index", model);
            }

            var client = _http.CreateClient("LoanityApi");
            HttpResponseMessage response;

            // RFID-only login: if username and password are empty, but RFID is present
            if (string.IsNullOrWhiteSpace(model.UserName) && string.IsNullOrWhiteSpace(model.PassWord) && !string.IsNullOrWhiteSpace(model.RfidChip))
            {
                // Call RFID login endpoint
                var rfidDto = new { RfidChip = model.RfidChip };
                response = await client.PostAsJsonAsync("api/auth/login-rfid", rfidDto);
            }
            else
            {
                // Hash password before sending to API
                model.PassWord = PasswordHelper.Hash(model.PassWord);
                response = await client.PostAsJsonAsync("api/auth/login", model);
            }

            if (response.IsSuccessStatusCode)
            {
                var loginResult = await response.Content.ReadFromJsonAsync<LoginResponse>();
                var user = loginResult.User;

                // save JWT somewhere (cookie/session)
                HttpContext.Session.SetString("JwtToken", loginResult.Token);

                // create claims (still cookies for MVC auth)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
                    new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Admin" : "User"),
                    new Claim("JwtToken", loginResult.Token) // Retrieve token later
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                TempData["ShowWelcome"] = true;  // Set TempData  for Hello message
                return RedirectToAction("Index", "Home");
            }

            // Read error message from API
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
