using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomerController> _logger;
        public CustomerController(HttpClient httpClient, ILogger<CustomerController> logger)
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
				new Customer { CustomerID = 4, FullName = "Tô Vĩnh Tiến", PhoneNumber = "0123456789", CCCD = "123456789", Email = "john@example.com" },
				new Customer { CustomerID = 5, FullName = "Trần Nhật Tân", PhoneNumber = "0987654321", CCCD = "987654321", Email = "jane@example.com" },
				new Customer { CustomerID = 6, FullName = "Alice Brown", PhoneNumber = "0912345678", CCCD = "456123789", Email = "alice@example.com" },
				new Customer { CustomerID = 7, FullName = "Tô Vĩnh Tiến", PhoneNumber = "0123456789", CCCD = "123456789", Email = "john@example.com" },
				new Customer { CustomerID = 8, FullName = "Trần Nhật Tân", PhoneNumber = "0987654321", CCCD = "987654321", Email = "jane@example.com" },
				new Customer { CustomerID = 9, FullName = "Alice Brown", PhoneNumber = "0912345678", CCCD = "456123789", Email = "alice@example.com" },
				new Customer { CustomerID = 10, FullName = "Tô Vĩnh Tiến", PhoneNumber = "0123456789", CCCD = "123456789", Email = "john@example.com" },
				new Customer { CustomerID = 11, FullName = "Trần Nhật Tân", PhoneNumber = "0987654321", CCCD = "987654321", Email = "jane@example.com" },
				new Customer { CustomerID = 12, FullName = "Alice Brown", PhoneNumber = "0912345678", CCCD = "456123789", Email = "alice@example.com" },
			};
		}


		public async Task<IActionResult> Index()
        {
            var role = HttpContext.Items["Role"]?.ToString();

            // Kiểm tra role và điều hướng đến view tương ứng
            if (role == "Admin")
            {
                return RedirectToAction("AdminCustomer");
            }
            else if (role == "Reception")
            {
                return RedirectToAction("ReceptionCustomer");
            }

            return View("Error"); // Nếu role không hợp lệ
        }

        public IActionResult AdminCustomer()
        {
			return View();
		}
        public IActionResult ReceptionCustomer()
        {
			var customers = FakeDatabase.Customers.ToList(); // Directly use the list of Customer objects
			return View(customers);
		}
    }
}
