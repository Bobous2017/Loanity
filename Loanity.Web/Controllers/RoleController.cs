using Loanity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.Web.Controllers
{
    public class RoleController : Controller
    {
        private readonly IHttpClientFactory _http;

        public RoleController(IHttpClientFactory http) => _http = http;

        public async Task<IActionResult> Read()
        {
            var client = _http.CreateClient("LoanityApi");
            var roles = await client.GetFromJsonAsync<List<Role>>("api/role");
            return View(roles ?? new());
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        {
            var client = _http.CreateClient("LoanityApi");
            var response = await client.PostAsJsonAsync("api/role", role);
            return response.IsSuccessStatusCode
                ? RedirectToAction("Read")
                : View(role);
        }

        public async Task<IActionResult> Update(int id)
        {
            var client = _http.CreateClient("LoanityApi");
            var role = await client.GetFromJsonAsync<Role>($"api/role/{id}");
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Role role)
        {
            var client = _http.CreateClient("LoanityApi");
            var response = await client.PutAsJsonAsync($"api/role/{id}", role);
            return response.IsSuccessStatusCode
                ? RedirectToAction("Read")
                : View(role);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _http.CreateClient("LoanityApi");
            var role = await client.GetFromJsonAsync<Role>($"api/role/{id}");
            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _http.CreateClient("LoanityApi");
            await client.DeleteAsync($"api/role/{id}");
            return RedirectToAction("Read");
        }

    }
}
