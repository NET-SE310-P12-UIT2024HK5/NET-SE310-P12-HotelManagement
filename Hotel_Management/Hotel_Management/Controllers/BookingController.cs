using Data.Models;
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

		public static class FakeDatabase
		{
			public static List<Customer> Customers { get; } = new List<Customer>
			{
				new Customer { CustomerID = 1, FullName = "Tô Vĩnh Tiến", PhoneNumber = "0123456789", CCCD = "123456789", Email = "john@example.com" },
				new Customer { CustomerID = 2, FullName = "Trần Nhật Tân", PhoneNumber = "0987654321", CCCD = "987654321", Email = "jane@example.com" },
				new Customer { CustomerID = 3, FullName = "Alice Brown", PhoneNumber = "0912345678", CCCD = "456123789", Email = "alice@example.com" },
			};

			public static List<Booking> Bookings { get; } = new List<Booking>
			{
				new Booking { BookingID = 1, CustomerID = 2, CheckInDate = DateTime.Now.AddDays(-1), CheckOutDate = DateTime.Now.AddDays(2), Status = "Confirmed" },
				new Booking { BookingID = 2, CustomerID = 2, CheckInDate = DateTime.Now, CheckOutDate = DateTime.Now.AddDays(3), Status = "Pending" },
				new Booking { BookingID = 3, CustomerID = 3, CheckInDate = DateTime.Now.AddDays(2), CheckOutDate = DateTime.Now.AddDays(5), Status = "Cancelled" },
				new Booking { BookingID = 4, CustomerID = 2, CheckInDate = DateTime.Now, CheckOutDate = DateTime.Now.AddDays(3), Status = "Pending" },
			};
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
            foreach (var booking in FakeDatabase.Bookings)
            {
                var customer = FakeDatabase.Customers.FirstOrDefault(c => c.CustomerID == booking.CustomerID);
                booking.CustomerName = customer?.FullName;
            }

            return View(FakeDatabase.Bookings);
        }
		public IActionResult ReceptionBooking()
		{
			foreach (var booking in FakeDatabase.Bookings)
			{
				var customer = FakeDatabase.Customers.FirstOrDefault(c => c.CustomerID == booking.CustomerID);
				booking.CustomerName = customer?.FullName;
			}

			return View(FakeDatabase.Bookings);
		}

		public IActionResult Edit()
		{
			return View();
		}

		[HttpPost]
		public IActionResult UpdateStatus(int bookingId, string status)
		{
			var booking = FakeDatabase.Bookings.FirstOrDefault(b => b.BookingID == bookingId);
			if (booking == null)
			{
				return Json(new { success = false, message = "Booking not found" });
			}

			booking.Status = status; // Cập nhật trạng thái
			return Json(new { success = true, message = "Status updated successfully", updatedStatus = status });
		}


	}
}
