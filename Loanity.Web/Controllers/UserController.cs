using Loanity.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Loanity.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _http;

        public UserController(IHttpClientFactory http) => _http = http;

        private async Task LoadRolesAsync()
        {
            var client = _http.CreateClient("LoanityApi");
            var roles = await client.GetFromJsonAsync<List<RoleDto>>("api/role");
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
        }

        public record RoleDto(int Id, string Name);


        public async Task<IActionResult> Read()
        {
            var client = _http.CreateClient("LoanityApi");
            var users = await client.GetFromJsonAsync<List<UserDto>>("api/user");

            return View(users ?? new());
        }

        public async Task<IActionResult> Create()
        {
            await LoadRolesAsync();
            return View("Create");
        }


        [HttpPost]
        public async Task<IActionResult> Create(UserDto user)
        {
            var client = _http.CreateClient("LoanityApi");
            var response = await client.PostAsJsonAsync("api/user", user);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Read");

            await LoadRolesAsync();
            return View();
        }


        public async Task<IActionResult> Update(int id)
        {
            var client = _http.CreateClient("LoanityApi");
            var user = await client.GetFromJsonAsync<UserDto>($"api/user/{id}");
            await LoadRolesAsync();
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, UserDto user)
        {
            user = user with { Id = id };
            var client = _http.CreateClient("LoanityApi");
            var response = await client.PutAsJsonAsync($"api/user/{id}", user);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Read");

            await LoadRolesAsync();
            return View("Update", user);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var client = _http.CreateClient("LoanityApi");
            var user = await client.GetFromJsonAsync<UserDto>($"api/user/{id}");
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _http.CreateClient("LoanityApi");
            await client.DeleteAsync($"api/user/{id}");
            return RedirectToAction("Read");
        }
    }

}
