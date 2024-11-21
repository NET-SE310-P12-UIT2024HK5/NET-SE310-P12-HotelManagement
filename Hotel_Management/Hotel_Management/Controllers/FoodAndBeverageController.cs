using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers
{
    public class FoodAndBeverageController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FoodAndBeverageController> _logger;
		private const int PageSize = 6;

		public FoodAndBeverageController(HttpClient httpClient, ILogger<FoodAndBeverageController> logger)
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
                return RedirectToAction("AdminFoodAndBeverage");
            }
            else if (role == "Reception")
            {
                return RedirectToAction("ReceptionFoodAndBeverage");
            }

            return View("Error"); // Nếu role không hợp lệ
        }

		public async Task<IActionResult> AdminFoodAndBeverage(int page = 1)
		{
			try
			{
				// Giả sử bạn lấy dữ liệu từ API
				var response = await _httpClient.GetAsync($"api/foodandbeverage?page={page}&pageSize={PageSize}");
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<PaginatedList<FoodAndBeverageItem>>();

					// Truyền dữ liệu phân trang vào ViewBag
					ViewBag.CurrentPage = page;
					ViewBag.TotalPages = result.TotalPages;
					ViewBag.HasPreviousPage = result.HasPreviousPage;
					ViewBag.HasNextPage = result.HasNextPage;
					ViewBag.TotalItems = result.TotalItems;

					return View(result.Items);
				}

				return View("Error");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching food and beverage items");
				return View("Error");
			}
		}
		public async Task<IActionResult> ReceptionFoodAndBeverage(int page = 1)
		{
			try
			{
				var response = await _httpClient.GetAsync($"api/foodandbeverage?page={page}&pageSize={PageSize}");
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<PaginatedList<FoodAndBeverageItem>>();

					ViewBag.CurrentPage = page;
					ViewBag.TotalPages = result.TotalPages;
					ViewBag.HasPreviousPage = result.HasPreviousPage;
					ViewBag.HasNextPage = result.HasNextPage;
					ViewBag.TotalItems = result.TotalItems;

					return View(result.Items);
				}

				return View("Error");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching food and beverage items");
				return View("Error");
			}
		}
	}
}
