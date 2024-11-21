using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers
{
    public class InvoiceController : Controller
    {
		private readonly HttpClient _httpClient;
		private readonly ILogger<InvoiceController> _logger;
		public InvoiceController(HttpClient httpClient, ILogger<InvoiceController> logger)
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
				return RedirectToAction("AdminInvoice");
			}
			else if (role == "Reception")
			{
				return RedirectToAction("ReceptionInvoice");
			}

			return View("Error"); // Nếu role không hợp lệ
		}

		public IActionResult AdminInvoice()
		{
			return View(); // Trả về danh sách sản phẩm cho Admin
		}
		public IActionResult ReceptionInvoice()
		{
			return View(); // Trả về danh sách sản phẩm cho Admin
		}
		public IActionResult Edit()
		{
			return View();
		}
		public IActionResult Create()
		{
			return View();
		}
	}
}
