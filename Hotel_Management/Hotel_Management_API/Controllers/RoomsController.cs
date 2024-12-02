using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Hotel_Management_API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class RoomsController : ControllerBase
    {
		private readonly ILogger<RoomsController> _logger;
		private readonly DatabaseContext _context;

		// Khởi tạo controller với dependency injection
		public RoomsController(ILogger<RoomsController> logger, DatabaseContext context)
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
				var canConnect = await _context.Rooms.FirstOrDefaultAsync();

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

		[HttpGet]
		public IActionResult Get()
		{
			try
			{
				var rooms = _context.Rooms
					.Select(r => new
					{
						r.RoomID,
						r.RoomNumber,
						r.RoomType,
						r.Price,
						r.MaxOccupancy,
						r.Status,
						r.Description
					});
				return Ok(rooms);
			}
			catch (Exception ex)
			{
				// Xử lý lỗi nếu có
				return StatusCode(500, new { message = ex.Message });
			}
		}        
    }    
}