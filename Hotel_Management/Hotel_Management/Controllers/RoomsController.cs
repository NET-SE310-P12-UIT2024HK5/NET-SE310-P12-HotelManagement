using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hotel_Management.Controllers
{
    public class RoomsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RoomsController> _logger;
        public RoomsController(HttpClient httpClient, ILogger<RoomsController> logger)
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
                return RedirectToAction("AdminRooms");
            }
            else if (role == "Reception")
            {
                return RedirectToAction("ReceptionRooms");
            }

            return View("Error"); // Nếu role không hợp lệ
        }

        public async Task<IActionResult> AdminRooms()
        {
            var rooms = await GetRoomsAsync();
            if (rooms == null)
            {
                return View("Error");
            }
            return View(rooms);
        }
        public async Task<IActionResult> ReceptionRooms()
        {
            var rooms = await GetRoomsAsync();
            if (rooms == null)
            {
                return View("Error");
            }
            return View(rooms);
        }

        public async Task<List<Rooms>?> GetRoomsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/Rooms");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error retrieving data from API.");
                    return null;
                }
                var rooms = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Rooms>>(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data from API.");
                return null;
            }
        }
    }
}
