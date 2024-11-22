using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hotel_Management.Controllers
{
    public class FoodAndBeverageController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FoodAndBeverageController> _logger;
		private const int pageSize = 6;

		public FoodAndBeverageController(HttpClient httpClient, ILogger<FoodAndBeverageController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private static List<FoodAndBeverageItem> _items = new List<FoodAndBeverageItem>
        {
		    new FoodAndBeverageItem { Id = 1, Name = "Pama Paw Salad1", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
			new FoodAndBeverageItem { Id = 2, Name = "Caesar Salad2", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
			new FoodAndBeverageItem { Id = 3, Name = "Greek Salad3", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
			new FoodAndBeverageItem { Id = 4, Name = "Cobb Salad4", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
			new FoodAndBeverageItem { Id = 5, Name = "Nicoise Salad5", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
			new FoodAndBeverageItem { Id = 6, Name = "Waldorf Salad6", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
			new FoodAndBeverageItem { Id = 7, Name = "7", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
			new FoodAndBeverageItem { Id = 8, Name = "8", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
            new FoodAndBeverageItem { Id = 9, Name = "d9", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
			new FoodAndBeverageItem { Id = 10, Name = "Waldorf Salad6", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
			new FoodAndBeverageItem { Id = 11, Name = "Waldorf Salad7", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
			new FoodAndBeverageItem { Id = 12, Name = "Waldorf Salad8", Price = 4150000, ImageUrl = "https://via.placeholder.com/150" },
			new FoodAndBeverageItem { Id = 13, Name = "Waldorf Salad9", Price = 150000, ImageUrl = "https://via.placeholder.com/150" },
		};
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

        public IActionResult AdminFoodAndBeverage(int page = 1)
        {


			var totalItems = _items.Count;
			var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

			var paginatedItems = _items
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;
			ViewBag.TotalItems = totalItems;
            ViewBag.IsReception = false;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				// Nếu là Ajax request, trả về partial view
				return PartialView("_FoodAndBeverageList", new
				{
					Items = paginatedItems,
					CurrentPage = page,
					TotalPages = totalPages,
					TotalItems = totalItems
				});
			}

			return View(paginatedItems);
		}
        public IActionResult ReceptionFoodAndBeverage(int page = 1)
        {
            _logger.LogInformation($"Requested Page: {page}");
            var totalItems = _items.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var paginatedItems = _items
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
            .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.IsReception = true;  // Flag để phân biệt view Reception

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_FoodAndBeverageList", new
                {
                    Items = paginatedItems,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalItems
                });
            }

            return View(paginatedItems);
        }
    }
}
