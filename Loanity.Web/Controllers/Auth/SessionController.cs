using Microsoft.AspNetCore.Mvc;
using System;

namespace Loanity.Web.Controllers.Auth
{
    public class SessionController : Controller
    {
        // Route: /session-expired
        [HttpGet("/session-expired")]
        public IActionResult Expired(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl ?? Url.Action("Index", "Home");
            return View(); // Views/Session/Expired.cshtml
        }
    }

}
