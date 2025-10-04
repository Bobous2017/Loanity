using Loanity.Domain.Dtos;
using Loanity.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Loanity.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly HttpClient _http;
    private readonly string _baseUrl;
    public HomeController(ILogger<HomeController> logger, IHttpClientFactory factory)
    {
        _logger = logger;
        _http = factory.CreateClient("LoanityAPI");
        _baseUrl = "http://localhost:5253/api/equipment/categories-with-quantity"; 
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        // Example check: redirect if not logged in
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Login");
        }

        var categories = await _http.GetFromJsonAsync<List<EquipmentCategoryDto>>($"{_baseUrl}");
        return View(categories);

    }

    public IActionResult Statistics()
    {
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
