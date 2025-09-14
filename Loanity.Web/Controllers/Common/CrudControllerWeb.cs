using Microsoft.AspNetCore.Mvc;

namespace Loanity.Web.Controllers.Common;

public abstract class CrudControllerWeb<TDto> : Controller where TDto : class
{
    protected readonly HttpClient _http;
    protected readonly string _baseUrl;

    protected CrudControllerWeb(IHttpClientFactory factory, string apiEndpoint)
    {
        _http = factory.CreateClient("LoanityApi");
        _baseUrl = $"api/{apiEndpoint}";
    }

    // READ
    public virtual async Task<IActionResult> Read()
    {
        var data = await _http.GetFromJsonAsync<List<TDto>>(_baseUrl);
        return View(data);
    }

    // CREATE - GET
    public virtual async Task<IActionResult> Create()
    {
        return View();
    }


    // CREATE - POST
    [HttpPost]
    public virtual async Task<IActionResult> Create(TDto dto)
    {
        var res = await _http.PostAsJsonAsync(_baseUrl, dto);
        return res.IsSuccessStatusCode ? RedirectToAction("Read") : View(dto);
    }

    // UPDATE - GET
    public virtual async Task<IActionResult> Update(int id)
    {
        var item = await _http.GetFromJsonAsync<TDto>($"{_baseUrl}/{id}");
        return View(item);
    }

    // UPDATE - POST
    [HttpPost]
    public virtual async Task<IActionResult> Update(int id, TDto dto)
    {
        var res = await _http.PutAsJsonAsync($"{_baseUrl}/{id}", dto);
        return res.IsSuccessStatusCode ? RedirectToAction("Read") : View(dto);
    }

    // DELETE - GET
    public virtual async Task<IActionResult> Delete(int id)
    {
        var item = await _http.GetFromJsonAsync<TDto>($"{_baseUrl}/{id}");
        return View(item);
    }

    // DELETE - POST
    [HttpPost, ActionName("Delete")]
    public virtual async Task<IActionResult> DeleteConfirmed(int id)
    {
        var res = await _http.DeleteAsync($"{_baseUrl}/{id}");
        return RedirectToAction("Read");
    }
}
