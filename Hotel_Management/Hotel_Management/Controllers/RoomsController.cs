using Microsoft.AspNetCore.Mvc;

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

        public IActionResult AdminRooms()
        {
            return View(); // Trả về danh sách sản phẩm cho Admin
        }
        public IActionResult ReceptionRooms()
        {
            return View(); // Trả về danh sách sản phẩm cho Admin
        }
    }
}
