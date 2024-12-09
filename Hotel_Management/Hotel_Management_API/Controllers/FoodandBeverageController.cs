using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Hotel_Management_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FoodandBeverageController : ControllerBase
    {
        private readonly ILogger<FoodandBeverageController> _logger;
        private readonly DatabaseContext _context;

        public FoodandBeverageController(ILogger<FoodandBeverageController> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("check-connection")]
        public async Task<IActionResult> CheckDatabaseConnection()
        {
            try
            {
                // Kiểm tra kết nối cơ sở dữ liệu bằng cách thử lấy dữ liệu từ bảng Booking
                var canConnect = await _context.FoodAndBeverageServices.FirstOrDefaultAsync();

                if (canConnect != null)
                {
                    return Ok("Kết nối cơ sở dữ liệu thành công!");
                }
                else
                {
                    return StatusCode(500, "Không thể truy vấn dữ liệu từ cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kết nối cơ sở dữ liệu");
                return StatusCode(500, $"Lỗi kết nối cơ sở dữ liệu: {ex.Message}");
            }
        }
        // API lấy danh sách tất cả các món ăn, đồ uống
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var foodandbeverages = await _context.FoodAndBeverageServices
                    .Where(f => f.IsAvailable)  // Có thể lọc theo điều kiện nếu cần
                    .Select(u => new
                    {
                        u.ServiceID,
                        u.ItemName,
                        u.ItemPrice,
                        u.Category,
                        u.Description,
                        IsAvailable = u.IsAvailable ? "Available" : "Unavailable"
                    })
                    .ToListAsync();

                return Ok(foodandbeverages);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching food and beverages: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching the data" });
            }
        }

    }
}
