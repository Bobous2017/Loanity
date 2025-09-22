using Loanity.Domain.Dtos;
using Loanity.Web.Controllers.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Loanity.Web.Controllers.Crud
{
    public class LoanController : CrudControllerWeb<LoanDto>
    {
        public LoanController(IHttpClientFactory factory)
            : base(factory, "loan") // maps to "api/loan"
        {
        }

        // You can optionally override Read() if you want custom error handling:
        public override async Task<IActionResult> Read()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<LoanDto>>(_baseUrl);
                return View(result ?? new List<LoanDto>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Could not load loans. API may be offline.";
                return View(new List<LoanDto>());
            }
        }
    }
}
