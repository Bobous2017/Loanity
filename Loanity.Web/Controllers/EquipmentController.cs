using Loanity.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
namespace Loanity.Web.Controllers;

public class EquipmentController : Controller
{
    private readonly IHttpClientFactory _http;

    public EquipmentController(IHttpClientFactory http)
    {
        _http = http;
    }

    //--------------------------------------CRUD - Equipment ------------------------------//
    public async Task<IActionResult> Read() //-------------------------------Read Fra API Get
    {
        var client = _http.CreateClient("LoanityApi");
        var equipments = await client.GetFromJsonAsync<List<EquipmentDto>>("api/equipment");

        return View(equipments ?? new List<EquipmentDto>()); //prevent null
    }


    public async Task<IActionResult> Create()
    {
        var client = _http.CreateClient("LoanityApi");
        var categories = await client.GetFromJsonAsync<List<CategoryDto>>("api/categories");

        ViewBag.Categories = categories;
        return View();
    }

    public record CategoryDto(int Id, string Name);


    // POST: /Equipment/Create
    [HttpPost]
    public async Task<IActionResult> Create(EquipmentDto dto)
    {
        var client = _http.CreateClient("LoanityApi");
        var response = await client.PostAsJsonAsync("api/equipment", dto);

        if (response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Read)); // back to list

        // Re-fetch categories in case of failure
        var categories = await client.GetFromJsonAsync<List<CategoryDto>>("api/categories");
        ViewBag.Categories = categories;

        ModelState.AddModelError("", "Failed to create equipment");
        return View(dto); // re-show form with dropdown intact
    }


    //GET: /Equipment/Update/{id}
    public async Task<IActionResult> Update(int id)
    {
        var client = _http.CreateClient("LoanityApi");

        // fetch equipment
        var equipment = await client.GetFromJsonAsync<EquipmentDto>($"api/equipment/{id}");

        // fetch categories
        var categories = await client.GetFromJsonAsync<List<CategoryDto>>("api/categories");
        ViewBag.Categories = categories;

        return View(equipment);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, EquipmentDto equipment)
    {
        var client = _http.CreateClient("LoanityApi");

        equipment = equipment with { Id = id };

        var response = await client.PutAsJsonAsync($"api/equipment/{id}", equipment);
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Update failed: {response.StatusCode}, {content}");

        if (response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Read));

        // re-fetch categories so view has them when redisplayed
        var categories = await client.GetFromJsonAsync<List<CategoryDto>>("api/categories");
        ViewBag.Categories = categories;

        return View(equipment);
    }




    // GET: /Equipment/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var client = _http.CreateClient("LoanityApi");
        var equipment = await client.GetFromJsonAsync<EquipmentDto>($"api/equipment/{id}");
        return View(equipment);
    }

    // POST: /Equipment/Delete/{id}
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = _http.CreateClient("LoanityApi");
        var response = await client.DeleteAsync($"api/equipment/{id}");
        return RedirectToAction(nameof(Read));
    }

}

