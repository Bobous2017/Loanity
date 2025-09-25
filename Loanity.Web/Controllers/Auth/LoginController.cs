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

        //[HttpGet("timer")]
        //public IActionResult GetTimer()
        //{
        //    var sessionTimer = SessionTimer.SessionTimeoutSeconds;
        //    var timer = new { num = sessionTimer };
        //    return Ok(timer);
        //}

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            //  Hash password before sending to API
            
            model.PassWord = PasswordHelper.Hash(model.PassWord);

            var client = _http.CreateClient("LoanityApi");
            var response = await client.PostAsJsonAsync("api/auth/login", model);

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

                return RedirectToAction("Index", "Home");
            }
            //ModelState.AddModelError("", "Invalid login attempt"); // Feeedback to user

            //  Read error message from API
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
