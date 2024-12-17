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
                    .Select(u => new
                    {
                        u.ServiceID,
                        u.ItemName,
                        u.ItemPrice,
                        u.Category,
                        u.Description,
                        u.ItemImage,
                        u.IsAvailable,
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

        // Phương thức POST để thêm mới một mục Food and Beverage
        [HttpPost]
        public async Task<IActionResult> AddFoodAndBeverage([FromBody] FoodAndBeverageServices newItem)
        {
            try
            {
                // Kiểm tra đầu vào
                if (newItem == null)
                {
                    return BadRequest(new { message = "Invalid item data" });
                }

                // Kiểm tra tên không được trống
                if (string.IsNullOrWhiteSpace(newItem.ItemName))
                {
                    return BadRequest(new { message = "Item name is required" });
                }

                // Kiểm tra giá không âm
                if (newItem.ItemPrice < 0)
                {
                    return BadRequest(new { message = "Price cannot be negative" });
                }

                // Đảm bảo ServiceID là 0 để tạo mới
                newItem.ServiceID = 0;

                // Thêm mới vào database
                _context.FoodAndBeverageServices.Add(newItem);
                await _context.SaveChangesAsync();

                // Trả về item vừa tạo kèm mã 201 Created
                return CreatedAtAction(nameof(Get), new { id = newItem.ServiceID }, newItem);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding food and beverage item: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while adding the item" });
            }
        }

        // Phương thức DELETE để xóa một mục Food and Beverage theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodAndBeverage(int id)
        {
            try
            {
                // Tìm item cần xóa
                var itemToDelete = await _context.FoodAndBeverageServices
                    .FirstOrDefaultAsync(item => item.ServiceID == id);

                // Kiểm tra nếu không tìm thấy
                if (itemToDelete == null)
                {
                    return NotFound(new { message = "Item not found" });
                }

                // Xóa item khỏi database
                _context.FoodAndBeverageServices.Remove(itemToDelete);
                await _context.SaveChangesAsync();

                // Trả về kết quả thành công
                return Ok(new { message = "Item deleted successfully", deletedItemId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting food and beverage item: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the item" });
            }
        }


        // Phương thức PUT để cập nhật một mục Food and Beverage theo ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFoodAndBeverage(int id, [FromBody] FoodAndBeverageServices updatedItem)
        {
            try
            {
                // Kiểm tra đầu vào
                if (updatedItem == null || id != updatedItem.ServiceID)
                {
                    return BadRequest(new { message = "Invalid request data" });
                }

                // Tìm mục cần cập nhật trong database
                var existingItem = await _context.FoodAndBeverageServices
                    .FirstOrDefaultAsync(item => item.ServiceID == id);

                // Kiểm tra nếu không tìm thấy mục
                if (existingItem == null)
                {
                    return NotFound(new { message = "Item not found" });
                }

                // Cập nhật các trường
                existingItem.ItemName = updatedItem.ItemName;
                existingItem.ItemPrice = updatedItem.ItemPrice;
                existingItem.Category = updatedItem.Category;
                existingItem.Description = updatedItem.Description;
                existingItem.IsAvailable = updatedItem.IsAvailable;

                // Lưu thay đổi vào database
                _context.FoodAndBeverageServices.Update(existingItem);
                await _context.SaveChangesAsync();

                // Trả về mục đã được cập nhật
                return Ok(new { message = "Item updated successfully", updatedItem = existingItem });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating food and beverage item: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the item" });
            }
        }

        // Phương thức GET để lấy chi tiết một mục Food and Beverage theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFoodAndBeverageById(int id)
        {
            try
            {
                // Tìm item theo ID
                var item = await _context.FoodAndBeverageServices
                    .FirstOrDefaultAsync(item => item.ServiceID == id);

                // Kiểm tra nếu không tìm thấy
                if (item == null)
                {
                    return NotFound(new { message = "Item not found" });
                }

                // Trả về thông tin item
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching food and beverage item: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching the item" });
            }
        }

        // Lấy danh sách Customer
        [HttpGet("booking")]
        public async Task<IActionResult> GetBooking()
        {
            try
            {
                var customers = await _context.Booking.ToListAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách Booking");
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFoodAndBeverage([FromForm] FoodAndBeverageServices newItem, [FromForm] IFormFile ItemImage)
        {
            try
            {
                if (newItem == null || ItemImage == null)
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ." });
                }

                using (var memoryStream = new MemoryStream())
                {
                    await ItemImage.CopyToAsync(memoryStream);
                    newItem.ItemImage = memoryStream.ToArray();
                }

                _context.FoodAndBeverageServices.Add(newItem);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Thêm mới thành công!", newItem });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm mới món ăn.");
                return StatusCode(500, new { message = "Có lỗi xảy ra.", error = ex.Message });
            }
        }

    }
}
