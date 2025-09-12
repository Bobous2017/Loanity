using Loanity.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _http;

        public CategoryController(IHttpClientFactory http) => _http = http;



        public async Task<IActionResult> Read()
        {
            var client = _http.CreateClient("LoanityApi");
            var categories = await client.GetFromJsonAsync<List<CategoryDto>>("api/categories");
            return View(categories ?? new List<CategoryDto>());
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto category)
        {
            var client = _http.CreateClient("LoanityApi");
            var response = await client.PostAsJsonAsync("api/categories", category);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Read));

            ModelState.AddModelError("", "Failed to create category");
            return View(category);
        }

        public async Task<IActionResult> Update(int id)
        {
            var client = _http.CreateClient("LoanityApi");
            var category = await client.GetFromJsonAsync<CategoryDto>($"api/categories/{id}");
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, CategoryDto category)
        {
            var client = _http.CreateClient("LoanityApi");
            var response = await client.PutAsJsonAsync($"api/categories/{id}", category);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Read));

            ModelState.AddModelError("", "Failed to update category");
            return View(category);
        }


        // GET: /Equipment/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var client = _http.CreateClient("LoanityApi");
            var equipment = await client.GetFromJsonAsync<CategoryDto>($"api/categories/{id}");
            return View(equipment);
        }

        // POST: /Equipment/Delete/{id}
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _http.CreateClient("LoanityApi");
            var response = await client.DeleteAsync($"api/categories/{id}");
            return RedirectToAction(nameof(Read));
        }
    }

}
