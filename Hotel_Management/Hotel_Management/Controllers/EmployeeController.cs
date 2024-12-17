using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hotel_Management.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EmployeeController> _logger;
        public EmployeeController(HttpClient httpClient, ILogger<EmployeeController> logger)
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
                return RedirectToAction("Employee");
            }
            return View("Error"); // Nếu role không hợp lệ
        }
        public async Task<IActionResult> Employee()
        {
            var employees = await GetEmployeesAsync();
            return View(employees);
        }

        public async Task<List<Users>> GetEmployeesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/Employee");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error retrieving data from API.");
                    return null;
                }

                var res = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Users>>(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching data from API.");
                return null;
            }
        }

        public async Task<IActionResult> CreateEmployee([FromBody] Users employee)
        {
            try
            {
                if (employee == null)
                {
                    return BadRequest(new { message = "Invalid employee data." });
                }

                // Gửi yêu cầu đến API
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7287/Employee", employee);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Employee created successfully." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating employee.");
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception while creating employee.");
                }
                return StatusCode(500, new { message = "An error occurred while creating the employee.", details = ex.InnerException?.Message });
            }
        }

        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                // Gửi yêu cầu xóa tới API
                var response = await _httpClient.DeleteAsync($"https://localhost:7287/Employee/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Employee deleted successfully." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting employee.");
                return StatusCode(500, new { message = "An error occurred while deleting the employee." });
            }
        }

        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Users employee)
        {
            try
            {
                if (employee == null)
                {
                    return BadRequest(new { message = "Invalid employee data." });
                }

                // Gửi yêu cầu cập nhật đến API
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7287/Employee/{id}", employee);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Employee updated successfully." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating employee.");
                return StatusCode(500, new { message = "An error occurred while updating the employee." });
            }
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
