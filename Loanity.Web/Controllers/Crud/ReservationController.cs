
using Loanity.Domain.Entities;
using Loanity.Web.Controllers.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Loanity.Web.Controllers
{
    public class ReservationController : CrudControllerWeb<Reservation>
    {
        private readonly HttpClient _http;
        private readonly string _actionBaseUrl;

        public ReservationController(IHttpClientFactory factory)
            : base(factory, "reservation")
        {
            _http = factory.CreateClient("LoanityApi");
            _actionBaseUrl = "api/reservation-action"; // This using Actions
        }

        // ---------------- OVERRIDE CREATE ----------------
        public override async Task<IActionResult> Create(){return View();}


        [HttpPost]
        public override async Task<IActionResult> Create(Reservation reservation)
        {
            var res = await _http.PostAsJsonAsync($"{_actionBaseUrl}/create", reservation);
            return res.IsSuccessStatusCode
                ? RedirectToAction("Read")
                : View(reservation);
        }

        // ---------------- OVERRIDE UPDATE ----------------
        public override async Task<IActionResult> Update(int id)
        {
            var reservation = await _http.GetFromJsonAsync<Reservation>($"{_baseUrl}/{id}");
            return View(reservation);
        }

        [HttpPost]
        public override async Task<IActionResult> Update(int id, Reservation reservation)
        {
            var res = await _http.PutAsJsonAsync($"{_actionBaseUrl}/update", reservation);
            return res.IsSuccessStatusCode
                ? RedirectToAction("Read")
                : View(reservation);
        }
    }
}
