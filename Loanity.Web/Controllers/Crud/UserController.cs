
using Loanity.Domain.Dtos;
using Loanity.Domain.Entities;
using Loanity.Web.Controllers.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;



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
        if (string.IsNullOrWhiteSpace(dto.PassWord))
        {
            dto = dto with { PassWord = null };
        }
        // Grab  User token  and  Send and Save for later to API
        var token = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        }
        
        var response = await _http.PutAsJsonAsync($"{_baseUrl}/{id}/dto", dto); // Send to 


        if (response.IsSuccessStatusCode)
            return RedirectToAction("Read");

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error); // Show admin password error
        }

        await LoadRolesAsync();
        return View(dto);
    }

}
