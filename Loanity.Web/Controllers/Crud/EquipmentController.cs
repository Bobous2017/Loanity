using Loanity.Domain.Dtos;
using Loanity.Domain.Dtos.CategoryType;
using Loanity.Web.Controllers.Common; // your base class
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace Loanity.Web.Controllers;

public class EquipmentController : CrudControllerWeb<EquipmentDto>
{
    public EquipmentController(IHttpClientFactory factory)
        : base(factory, "equipment")
    {
    }


    // ---------------------Get first  Categorys for dropdown-----
    private async Task LoadCategories()
    {
        var categories = await _http.GetFromJsonAsync<List<CategoryDto>>("api/categories");
        ViewBag.Categories = categories;
    }
    // ------------- Custom Create View (with dropdown)
    public override async Task<IActionResult> Create()
    {
        await LoadCategories();
        return View();
    }

    [HttpPost]
    public override async Task<IActionResult> Create(EquipmentDto dto)
    {
        var result = await base.Create(dto);

        if (result is RedirectToActionResult)
        {
            TempData["Success"] = "Equipment was successfully created!";
            return result;
        }

        TempData["Error"] = "Failed to create equipment. Please check your input.";
        await LoadCategories();
        return View(dto);
    }


    // ------------- Custom Update View (with dropdown)
    public override async Task<IActionResult> Update(int id)
    {
        await LoadCategories();
        return await base.Update(id);
    }

    [HttpPost]
    public override async Task<IActionResult> Update(int id, EquipmentDto dto)
    {
        var result = await base.Update(id, dto);

        if (result is RedirectToActionResult)
        {
            TempData["Success"] = "Equipment was updated successfully!";
            return result;
        }

        TempData["Error"] = "Failed to update equipment. Please check your input.";
        await LoadCategories();
        return View(dto);
    }

    public async Task<IActionResult> ByCategory(string name)
    {
        var equipment = await _http.GetFromJsonAsync<List<EquipmentDto>>(
            $"{_baseUrl}/category/{name}");

        ViewBag.CategoryName = name;
        return View(equipment);  // ← this is a list!
    }




}
