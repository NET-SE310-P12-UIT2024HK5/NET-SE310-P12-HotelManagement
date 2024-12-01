using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

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
            return View(bookings); // Truyền danh sách booking vào view
        }

		public async Task<IActionResult> ReceptionBooking()
		{
            var bookings = await GetBookingsAsync();
            return View(bookings); // Truyền danh sách booking vào view
        }

        private async Task<List<Booking>> GetBookingsAsync()
        {
            try
            {
                // Gọi API để lấy danh sách booking
                var response = await _httpClient.GetAsync("https://localhost:7287/Booking");

                // Kiểm tra nếu response không thành công
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Lỗi khi gọi API: {response.StatusCode}");
                    return new List<Booking>(); // Trả về danh sách rỗng nếu có lỗi
                }

                // Đọc dữ liệu JSON từ response
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserialze dữ liệu JSON thành danh sách đối tượng Booking
                var bookings = JsonConvert.DeserializeObject<List<Booking>>(jsonString);

                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy danh sách booking: {ex.Message}");
                return new List<Booking>(); // Trả về danh sách rỗng trong trường hợp có lỗi
            }
        }



    }
}
