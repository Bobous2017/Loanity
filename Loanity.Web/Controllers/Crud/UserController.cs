
using Loanity.Web.Controllers.Common;
using Loanity.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Loanity.Domain.Entities;

namespace Loanity.Web.Controllers;

public class UserController : CrudControllerWeb<UserDto>
{
    public UserController(IHttpClientFactory factory)
        : base(factory, "user") { }

    private async Task LoadRolesAsync()
    {
        var roles = await _http.GetFromJsonAsync<List<Role>>("api/role");
        ViewBag.Roles = new SelectList(roles, "Id", "Name");
    }

    //public record RoleDto(int Id, string Name);

    public override async Task<IActionResult> Create()
    {
        await LoadRolesAsync();
        return View();
    }

    [HttpPost]
    public override async Task<IActionResult> Create(UserDto dto)
    {
        var response = await _http.PostAsJsonAsync(_baseUrl, dto);
        if (response.IsSuccessStatusCode)
            return RedirectToAction("Read");

        await LoadRolesAsync();
        return View(dto);
    }

    public override async Task<IActionResult> Update(int id)
    {
        var user = await _http.GetFromJsonAsync<UserDto>($"{_baseUrl}/{id}");
        await LoadRolesAsync();
        return View(user);
    }

    [HttpPost]
    public override async Task<IActionResult> Update(int id, UserDto dto)
    {
        dto = dto with { Id = id };
        var response = await _http.PutAsJsonAsync($"{_baseUrl}/{id}", dto);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Read");

        await LoadRolesAsync();
        return View(dto);
    }
}
