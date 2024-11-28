using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hotel_Management.Controllers
{
    public class BookingController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookingController> _logger;

        public BookingController(HttpClient httpClient, ILogger<BookingController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Items["Role"]?.ToString();

            // Kiểm tra role và điều hướng đến view tương ứng
            if (role == "Admin")
            {
                return RedirectToAction("AdminBooking");
            }
            else if (role == "Reception")
            {
                return RedirectToAction("ReceptionBooking");
            }

            return View("Error"); // Nếu role không hợp lệ
        }

        public async Task<IActionResult> AdminBooking()
        {
            var bookings = await GetBookingsAsync();
            if (bookings == null) return View("Error");
            return View(bookings);
        }

        public async Task<IActionResult> ReceptionBooking()
        {
            var bookings = await GetBookingsAsync();
            if (bookings == null) return View("Error");
            return View(bookings);
        }

        private async Task<List<Booking>> GetBookingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/Booking");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error retrieving data from API.");
                    return null;
                }

                var res = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Booking>>(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching data from API.");
                return null;
            }
        }

        public async Task<IActionResult> UpdateStatus(int bookingId, string status)
        {
            return Ok(status);
        }
    }
}
