//// -------------------- ReservationController.cs (Loanity.Web) --------------------
//using Loanity.Domain.Entities;
//using Microsoft.AspNetCore.Mvc;
//using System.Net.Http.Json;

//namespace Loanity.Web.Controllers.Crud
//{
//    public class ReservationController : Controller
//    {
//        private readonly HttpClient _http;
//        private readonly string _baseUrl = "http://localhost:5253/api/reservation";
//        private string _baseUrlAction = "http://localhost:5253/api/reservation-action";
//        public ReservationController(HttpClient http)
//        {
//            _http = http;
//        }

//        // READ
//        public async Task<IActionResult> Read()
//        {
//            var reservations = await _http.GetFromJsonAsync<List<Reservation>>(_baseUrl);
//            return View(reservations);
//        }

//        // CREATE - GET
//        public IActionResult Create() => View();

//        // CREATE - POST
//        [HttpPost]
//        public async Task<IActionResult> Create(Reservation reservation)
//        {
//            var res = await _http.PostAsJsonAsync($"{_baseUrlAction}/create", reservation);
//            if (res.IsSuccessStatusCode)
//                return RedirectToAction("Read");
//            return View(reservation);
//        }

//        // UPDATE - GET
//        public async Task<IActionResult> Update(int id)
//        {
//            var reservation = await _http.GetFromJsonAsync<Reservation>($"{_baseUrl}/{id}");
//            return View(reservation);
//        }

//        // UPDATE - POST
//        [HttpPost]
//        public async Task<IActionResult> Update(int id, Reservation reservation)
//        {
//            var res = await _http.PutAsJsonAsync($"{_baseUrlAction}/update", reservation);
//            if (res.IsSuccessStatusCode)
//                return RedirectToAction("Read");

//            return View(reservation);
//        }

//        // DELETE - GET
//        public async Task<IActionResult> Delete(int id)
//        {
//            var reservation = await _http.GetFromJsonAsync<Reservation>($"{_baseUrl}/{id}");
//            return View(reservation);
//        }

//        // DELETE - POST
//        [HttpPost, ActionName("Delete")]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var res = await _http.DeleteAsync($"{_baseUrl}/{id}");
//            return RedirectToAction("Read");
//        }
//    }
//}


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
