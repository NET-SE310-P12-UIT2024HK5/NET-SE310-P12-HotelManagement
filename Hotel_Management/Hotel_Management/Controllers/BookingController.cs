using Microsoft.AspNetCore.Mvc;

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

        public IActionResult AdminBooking()
        {
            return View(); // Trả về danh sách sản phẩm cho Admin
        }
        public IActionResult ReceptionBooking()
        {
            return View(); // Trả về danh sách sản phẩm cho Admin
        }
    }
}
