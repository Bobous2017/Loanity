using Loanity.Domain.Dtos.UserHandlingDto;
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
        {
            TempData["Success"] = "User was created successfully!";
            return RedirectToAction("Read");
        }

        TempData["Error"] = "Failed to create user. Please try again.";
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

        // Send token to API
        var token = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _http.PutAsJsonAsync($"{_baseUrl}/{id}/dto", dto);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "User updated successfully!";
            return RedirectToAction("Read");
        }

        // Handle specific error
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error); // Show admin password error
        }
        else
        {
            TempData["Error"] = "Failed to update user. Please check your inputs.";
        }

        await LoadRolesAsync();
        return View(dto);
    }

    public async Task<IActionResult> Details(int id)
    {
        
        var result = await _http.GetFromJsonAsync<List<UserLoanDto>>($"api/user/user-loans/{id}");

        return View(result ?? new());
    }


}
