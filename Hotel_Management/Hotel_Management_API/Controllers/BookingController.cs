using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Hotel_Management_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly DatabaseContext _context;

        // Khởi tạo controller với dependency injection
        public BookingController(ILogger<BookingController> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Phương thức để kiểm tra kết nối đến cơ sở dữ liệu
        [HttpGet("check-connection")]
        public async Task<IActionResult> CheckDatabaseConnection()
        {
            try
            {
                // Kiểm tra kết nối cơ sở dữ liệu bằng cách thử lấy dữ liệu từ bảng Booking
                var canConnect = await _context.Booking.FirstOrDefaultAsync();

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

		// Phương thức lấy tất cả booking
		[HttpGet]
		public IActionResult Get()
		{
			try
			{
				// Lấy dữ liệu từ DbContext
				var bookings = _context.Booking
					.Include(b => b.Customer)  // Bao gồm thông tin khách hàng
					.Include(b => b.Room)      // Bao gồm thông tin phòng
					.Select(b => new
					{
						b.BookingID,
						b.CustomerID,
						CustomerName = b.Customer.FullName,  // Lấy tên khách hàng từ Customer
						b.RoomID,
						RoomNumber = b.Room.RoomNumber,     // Lấy số phòng
						b.CheckInDate,
						b.CheckOutDate,
						b.Status
					})
					.ToList();

				// Trả về danh sách booking dưới dạng JSON
				return Ok(bookings);
			}
			catch (Exception ex)
			{
				// Xử lý lỗi nếu có
				return StatusCode(500, new { message = ex.Message });
			}
		}

	}
}
