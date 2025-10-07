
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
        public override async Task<IActionResult> Read()
        {
            var reservations = await _http.GetFromJsonAsync<List<Reservation>>(_baseUrl);
            // Sort by StartAt descending
            var sorted = reservations
                .OrderByDescending(r => r.StartAt)
                .ToList();

            return View(sorted);
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
        // ------help for dropdowns------
        public override async Task<IActionResult> Update(int id)
        {
            var reservation = await _http.GetFromJsonAsync<Reservation>($"{_baseUrl}/{id}");
            return View(reservation);
        }


        [HttpPost]
        public override async Task<IActionResult> Update(int id, Reservation reservation)
        {
            var res = await _http.PutAsJsonAsync($"{_actionBaseUrl}/update", reservation);

            if (res.IsSuccessStatusCode)
            {
                TempData["Success"] = "Reservation was updated successfully!";
                return RedirectToAction("Read");
            }

            TempData["Error"] = "Failed to update reservation. Please review the form.";
            return View(reservation);
        }

    }
}
