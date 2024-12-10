using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Hotel_Management.Models;
using Azure;

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

        /*public async Task<IActionResult> AdminFoodAndBeverage()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/FoodandBeverage");

                if (!response.IsSuccessStatusCode)
                {
                    // Log chi tiết mã trạng thái
                    _logger.LogError($"API Error: {response.StatusCode}");
                    return View("Error", new ErrorViewModel
                    {
                        Message = $"Unable to fetch data. Status code: {response.StatusCode}"
                    });
                }

                var res = await response.Content.ReadAsStringAsync();

                // Log nội dung response để kiểm tra
                _logger.LogInformation($"API Response: {res}");

                var foodAndBeverageList = JsonConvert.DeserializeObject<List<FoodAndBeverageServices>>(res);

                // Kiểm tra nếu danh sách rỗng
                if (foodAndBeverageList == null || !foodAndBeverageList.Any())
                {
                    _logger.LogWarning("No food and beverage items found");
                    return View("Error", new ErrorViewModel
                    {
                        Message = "No food and beverage items found"
                    });
                }

                return View(foodAndBeverageList);
            }
            catch (JsonException jsonEx)
            {
                // Lỗi chuyển đổi JSON
                _logger.LogError($"JSON Deserialize Error: {jsonEx.Message}");
                return View("Error", new ErrorViewModel
                {
                    Message = $"Error parsing data: {jsonEx.Message}"
                });
            }
            catch (HttpRequestException httpEx)
            {
                // Lỗi kết nối HTTP
                _logger.LogError($"HTTP Request Error: {httpEx.Message}");
                return View("Error", new ErrorViewModel
                {
                    Message = $"Network error: {httpEx.Message}"
                });
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết lỗi
                _logger.LogError(ex, "Unexpected error in AdminFoodAndBeverage");
                return View("Error", new ErrorViewModel
                {
                    Message = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }*/

        public async Task<IActionResult> AdminFoodAndBeverage(int page = 1)
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/FoodandBeverage");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Error: {response.StatusCode}");
                    return View("Error", new ErrorViewModel
                    {
                        Message = $"Unable to fetch data. Status code: {response.StatusCode}"
                    });
                }

                var res = await response.Content.ReadAsStringAsync();
                var foodAndBeverageList = JsonConvert.DeserializeObject<List<FoodAndBeverageServices>>(res);

                if (foodAndBeverageList == null || !foodAndBeverageList.Any())
                {
                    _logger.LogWarning("No food and beverage items found");
                    return View("Error", new ErrorViewModel
                    {
                        Message = "No food and beverage items found"
                    });
                }

                // Phân trang
                int totalItems = foodAndBeverageList.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var pagedItems = foodAndBeverageList
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalItems = totalItems;
                ViewBag.IsReception = false;

                // Kiểm tra nếu là AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_FoodandBeverageList", pagedItems);
                }

                // Nếu không phải, trả về view đầy đủ
                return View(pagedItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AdminFoodAndBeverage");
                return View("Error", new ErrorViewModel
                {
                    Message = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }



        public async Task<IActionResult> ReceptionFoodAndBeverage(int page = 1)
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/FoodandBeverage");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Error: {response.StatusCode}");
                    return View("Error", new ErrorViewModel
                    {
                        Message = $"Unable to fetch data. Status code: {response.StatusCode}"
                    });
                }

                var res = await response.Content.ReadAsStringAsync();
                var foodAndBeverageList = JsonConvert.DeserializeObject<List<FoodAndBeverageServices>>(res);

                if (foodAndBeverageList == null || !foodAndBeverageList.Any())
                {
                    _logger.LogWarning("No food and beverage items found");
                    return View("Error", new ErrorViewModel
                    {
                        Message = "No food and beverage items found"
                    });
                }

                // Phân trang
                int totalItems = foodAndBeverageList.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var pagedItems = foodAndBeverageList
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalItems = totalItems;
                ViewBag.IsReception = true;

                // Kiểm tra nếu là AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_FoodandBeverageList", pagedItems);
                }

                // Nếu không phải, trả về view đầy đủ
                return View(pagedItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AdminFoodAndBeverage");
                return View("Error", new ErrorViewModel
                {
                    Message = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
