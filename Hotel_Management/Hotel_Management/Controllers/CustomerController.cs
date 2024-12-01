using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        public async Task<IActionResult> AdminCustomer()
        {
            var customers = await GetCustomersAsync();
            if (customers == null)
            {
                return View("Error");
            }
            return View(customers);
        }
        public async Task<IActionResult> ReceptionCustomer()
        {
			var customers = await GetCustomersAsync();
			if (customers == null)
			{
				return View("Error");
			}
			return View(customers);
		}

        public async Task<List<Customer>> GetCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/Customer");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error retrieving data from API.");
                    return null;
                }

                var res = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Customer>>(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching data from API.");
                return null;
            }
        }

        
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            try
            {
                if (customer == null)
                {
                    return BadRequest(new { message = "Invalid customer data." });
                }

                // Gửi yêu cầu đến API
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7287/Customer", customer);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Customer created successfully." });
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
                return StatusCode(500, new { message = "An error occurred while creating the customer." });
            }
        }

		public async Task<IActionResult> DeleteCustomer(int id)
		{
			try
			{
				// Gửi yêu cầu xóa tới API
				var response = await _httpClient.DeleteAsync($"https://localhost:7287/Customer/{id}");

				if (response.IsSuccessStatusCode)
				{
					return Ok(new { message = "Customer deleted successfully." });
				}
				else
				{
					var errorResponse = await response.Content.ReadAsStringAsync();
					return StatusCode((int)response.StatusCode, new { message = errorResponse });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while deleting customer.");
				return StatusCode(500, new { message = "An error occurred while deleting the customer." });
			}
		}

		public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
		{
			try
			{
				if (customer == null)
				{
					return BadRequest(new { message = "Invalid customer data." });
				}

				// Gửi yêu cầu cập nhật đến API
				var response = await _httpClient.PutAsJsonAsync($"https://localhost:7287/Customer/{id}", customer);

				if (response.IsSuccessStatusCode)
				{
					return Ok(new { message = "Customer updated successfully." });
				}
				else
				{
					var errorResponse = await response.Content.ReadAsStringAsync();
					return StatusCode((int)response.StatusCode, new { message = errorResponse });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while updating customer.");
				return StatusCode(500, new { message = "An error occurred while updating the customer." });
			}
		}

		public IActionResult Edit()
        {
            return View();
        }
    }
}
