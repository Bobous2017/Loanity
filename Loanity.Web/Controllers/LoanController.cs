using Loanity.Domain.Dtos;
using Loanity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;

namespace Loanity.Web.Controllers
{
    public class LoanController : Controller
    {
        private readonly IHttpClientFactory _http;
        //private readonly string _baseUrl = "http://localhost:5253/api/loan";

        public LoanController(IHttpClientFactory http)
        {
            _http = http;
        }

        // READ

        public async Task<IActionResult> Read()
        {
            try
            {
                var client = _http.CreateClient("LoanityApi");
                var loans = await client.GetFromJsonAsync<List<LoanDto>>("api/loan");
                return View(loans ?? new List<LoanDto>());

            }
            catch (HttpRequestException ex)
            {
                // Optional: log ex
                ViewBag.Error = "Could not load loans. API may be offline.";
                return View(new List<LoanDto>());
            }
        }


    }
}
