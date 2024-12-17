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

                var bookings = await GetBookingAsync();
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
                ViewBag.Bookings = bookings;

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

        public async Task<IActionResult> CreateFoodAndBeverage([FromBody] FoodAndBeverageServices foodAndBeverageServices)
        {
            try
            {
                if (foodAndBeverageServices == null)
                {
                    return BadRequest(new { message = "Invalid Food and Beverage data." });
                }

                // Gửi yêu cầu đến API
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7287/FoodandBeverage", foodAndBeverageServices);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Food and beverage created successfully." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating customer.");
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception while creating customer.");
                }
                return StatusCode(500, new { message = "An error occurred while creating the customer.", details = ex.InnerException?.Message });
            }
        }

        public async Task<IActionResult> DeleteFoodAndBeverage(int id)
        {
            try
            {
                // Gửi yêu cầu DELETE đến API
                var response = await _httpClient.DeleteAsync($"https://localhost:7287/FoodandBeverage/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    // Đọc nội dung lỗi để debug
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API Error: {response.StatusCode}, Error: {errorContent}");

                    return Json(new
                    {
                        success = false,
                        message = $"Không thể xóa. Mã lỗi: {response.StatusCode}, Chi tiết: {errorContent}"
                    });
                }

                // Nếu xóa thành công
                return Json(new { success = true, message = "Xóa thành công" });
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết lỗi
                _logger.LogError(ex, "Lỗi không mong muốn khi xóa");
                return Json(new
                {
                    success = false,
                    message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> EditFoodAndBeverage(int id)
        {
            try
            {
                // Gửi yêu cầu GET đến API để lấy chi tiết mục
                var response = await _httpClient.GetAsync($"https://localhost:7287/FoodandBeverage/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Error: {response.StatusCode}");
                    return Json(new
                    {
                        success = false,
                        message = $"Không thể lấy thông tin. Mã lỗi: {response.StatusCode}"
                    });
                }

                var res = await response.Content.ReadAsStringAsync();
                var foodItem = JsonConvert.DeserializeObject<FoodAndBeverageServices>(res);

                if (foodItem == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Không tìm thấy mục"
                    });
                }

                return Json(new
                {
                    success = true,
                    item = foodItem
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không mong muốn khi lấy thông tin để chỉnh sửa");
                return Json(new
                {
                    success = false,
                    message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> UpdateFoodAndBeverage([FromBody] FoodAndBeverageServices foodAndBeverageServices)
        {
            try
            {
                // Gửi yêu cầu PUT đến API
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7287/FoodandBeverage/{foodAndBeverageServices.ServiceID}", foodAndBeverageServices);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Cập nhật thành công"
                    });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new
                    {
                        success = false,
                        message = errorResponse
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        private async Task<List<Booking>> GetBookingAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/FoodandBeverage/booking");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Lỗi khi gọi API: {response.StatusCode}");
                    return new List<Booking>();
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var bookings = JsonConvert.DeserializeObject<List<Booking>>(jsonString);

                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy danh sách Customer: {ex.Message}");
                return new List<Booking>();
            }
        }

		[HttpPost]
		public async Task<IActionResult> SubmitOrder([FromBody] SubmitOrderModel orderRequest)
		{
			try
			{
				if (orderRequest == null || orderRequest.OrderItems == null || !orderRequest.OrderItems.Any())
				{
					return BadRequest(new
					{
						success = false,
						message = "Dữ liệu đơn hàng không hợp lệ."
					});
				}

				var response = await _httpClient.PostAsJsonAsync("https://localhost:7287/BookingFoodServiceDetails/SubmitOrder", orderRequest);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadAsStringAsync();
					return Ok(new
					{
						success = true,
						message = "Đơn hàng đã được gửi thành công!",
						data = result
					});
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("API Error: {ErrorContent}", errorContent);
					return StatusCode((int)response.StatusCode, new
					{
						success = false,
						message = errorContent
					});
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while submitting order");
				return StatusCode(500, new
				{
					success = false,
					message = $"Lỗi không mong muốn: {ex.Message}"
				});
			}
		}

        public async Task<IActionResult> SearchByBookingId(int bookingId)
        {
            try
            {
                // Gửi yêu cầu GET đến API để tìm kiếm theo Booking ID
                var response = await _httpClient.GetAsync($"https://localhost:7287/BookingFoodServices/by-booking/{bookingId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Error: {response.StatusCode}");
                    return Json(new
                    {
                        success = false,
                        message = $"Không thể tìm thấy dịch vụ. Mã lỗi: {response.StatusCode}"
                    });
                }

                var res = await response.Content.ReadAsStringAsync();
                var bookingFoodService = JsonConvert.DeserializeObject<BookingFoodServices>(res);

                if (bookingFoodService == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Không tìm thấy dịch vụ đặt thức ăn cho Booking ID này"
                    });
                }

                return Json(new
                {
                    success = true,
                    data = bookingFoodService
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không mong muốn khi tìm kiếm dịch vụ đặt thức ăn");
                return Json(new
                {
                    success = false,
                    message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> DeleteFoodServiceDetail(int detailId)
        {
            try
            {
                // Gửi yêu cầu DELETE đến API
                var response = await _httpClient.DeleteAsync($"https://localhost:7287/BookingFoodServiceDetails/DeleteDetail/{detailId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API Error: {response.StatusCode}, Error: {errorContent}");

                    return Json(new
                    {
                        success = false,
                        message = $"Không thể xóa. Mã lỗi: {response.StatusCode}, Chi tiết: {errorContent}"
                    });
                }

                // Đọc nội dung phản hồi
                var result = await response.Content.ReadAsStringAsync();
                var deleteResult = JsonConvert.DeserializeObject<dynamic>(result);

                // Nếu xóa thành công
                return Json(new
                {
                    success = true,
                    message = "Xóa thành công",
                    totalPrice = deleteResult.totalPrice,
                    bookingFoodServiceId = deleteResult.bookingFoodServiceId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không mong muốn khi xóa chi tiết dịch vụ");
                return Json(new
                {
                    success = false,
                    message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }


    }
}
