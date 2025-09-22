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
            return result;

        // Failed → re-fetch categories
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
            return result;

        await LoadCategories();
        return View(dto);
    }

   
}
