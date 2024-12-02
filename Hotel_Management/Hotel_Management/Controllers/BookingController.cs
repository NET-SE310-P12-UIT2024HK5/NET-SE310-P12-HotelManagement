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

            var customers = await GetCustomersAsync();
            var rooms = await GetRoomsAsync();
            ViewBag.Customers = customers;
            ViewBag.Rooms = rooms;

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

        private async Task<List<Customer>> GetCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/Booking/customers");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Lỗi khi gọi API: {response.StatusCode}");
                    return new List<Customer>();
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(jsonString);

                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy danh sách Customer: {ex.Message}");
                return new List<Customer>();
            }
        }

        private async Task<List<Rooms>> GetRoomsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/Booking/rooms");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Lỗi khi gọi API: {response.StatusCode}");
                    return new List<Rooms>();
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<Rooms>>(jsonString);

                return rooms;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy danh sách Room: {ex.Message}");
                return new List<Rooms>();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            try
            {
                if (booking == null)
                {
                    return BadRequest(new { message = "Invalid booking data." });
                }

                // Gửi yêu cầu đến API
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7287/Booking", booking);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Booking created successfully." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating booking");
                return StatusCode(500, new { message = "An error occurred while creating the booking.", details = ex.Message });
            }
        }

        // Xoá booking
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"https://localhost:7287/Booking/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Booking deleted successfully." });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting booking: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the booking." });
            }
        }

        //Update Booking
        [HttpPost]
        public async Task<IActionResult> UpdateBooking([FromBody] Booking updatedBooking)
        {
            try
            {
                if (updatedBooking == null)
                {
                    return BadRequest(new { message = "Invalid booking data." });
                }

                // Debug: In ra thông tin booking nhận được
                _logger.LogInformation($"Received booking update: {JsonConvert.SerializeObject(updatedBooking)}");

                // Gửi yêu cầu PUT đến API backend
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7287/Booking/{updatedBooking.BookingID}", updatedBooking);

                // Debug: In ra response từ API
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Booking updated successfully.", data = responseContent });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { message = responseContent });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating booking");
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the booking.",
                    details = ex.Message
                });
            }
        }


    }
}
