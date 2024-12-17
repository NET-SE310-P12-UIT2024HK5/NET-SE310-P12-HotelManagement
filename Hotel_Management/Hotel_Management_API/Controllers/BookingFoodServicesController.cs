using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Hotel_Management_API.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class BookingFoodServicesController : ControllerBase
	{
		private readonly DatabaseContext _context;
		private readonly ILogger<BookingFoodServicesController> _logger;

		public BookingFoodServicesController(DatabaseContext context, ILogger<BookingFoodServicesController> logger)
		{
			_context = context;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Get()
		{
			var services = _context.BookingFoodServices.ToList();
			return Ok(services);
		}

		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var service = _context.BookingFoodServices
				.Include(s => s.BookingFoodServiceDetails)
				.FirstOrDefault(s => s.BookingFoodServiceID == id);

			if (service == null)
			{
				_logger.LogWarning("BookingFoodService with ID {Id} not found.", id);
				return NotFound();
			}
			return Ok(service);
		}

		[HttpPost]
		public IActionResult Create([FromBody] BookingFoodServices bookingFoodService)
		{
			if (bookingFoodService == null)
			{
				_logger.LogWarning("Invalid BookingFoodService data received.");
				return BadRequest("Invalid data.");
			}

			_context.BookingFoodServices.Add(bookingFoodService);
			_context.SaveChanges();

			_logger.LogInformation("BookingFoodService created with ID {Id}", bookingFoodService.BookingFoodServiceID);
			return CreatedAtAction(nameof(GetById), new { id = bookingFoodService.BookingFoodServiceID }, bookingFoodService);
		}

		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] BookingFoodServices bookingFoodService)
		{
			var existing = _context.BookingFoodServices.FirstOrDefault(s => s.BookingFoodServiceID == id);
			if (existing == null)
			{
				_logger.LogWarning("BookingFoodService with ID {Id} not found.", id);
				return NotFound();
			}

			existing.TotalPrice = bookingFoodService.TotalPrice;
			existing.OrderTime = bookingFoodService.OrderTime;

			_context.SaveChanges();
			_logger.LogInformation("BookingFoodService with ID {Id} updated.", id);

			return NoContent();
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var existing = _context.BookingFoodServices.FirstOrDefault(s => s.BookingFoodServiceID == id);
			if (existing == null)
			{
				_logger.LogWarning("BookingFoodService with ID {Id} not found.", id);
				return NotFound();
			}

			_context.BookingFoodServices.Remove(existing);
			_context.SaveChanges();

			_logger.LogInformation("BookingFoodService with ID {Id} deleted.", id);
			return NoContent();
		}

		[HttpGet("by-booking/{bookingId}")]
		public IActionResult GetByBookingId(int bookingId)
		{
			// Lấy BookingFoodService cùng các chi tiết liên quan
			var service = _context.BookingFoodServices
				.Include(bfs => bfs.BookingFoodServiceDetails)          // Bao gồm danh sách các chi tiết
				.ThenInclude(detail => detail.FoodAndBeverageService)  // Bao gồm thông tin dịch vụ chi tiết (nếu cần)
				.FirstOrDefault(s => s.BookingID == bookingId);     // Lọc theo ID

			// Kiểm tra nếu không tìm thấy
			if (service == null)
			{
				_logger.LogWarning("BookingFoodService with ID {Id} not found.", bookingId);
				return NotFound(new { message = $"BookingFoodService with ID {bookingId} not found." });
			}

			// Trả về thông tin chi tiết
			_logger.LogInformation("Details Count: {count}", service?.BookingFoodServiceDetails?.Count);
			return Ok(service);
		}

        [HttpDelete("detail/{detailId}")]
        public IActionResult DeleteDetail(int detailId)
        {
            // Tìm BookingFoodServiceDetail theo detailId
            var detail = _context.BookingFoodServiceDetails.FirstOrDefault(d => d.BookingFoodServiceDetailID == detailId);

            // Kiểm tra nếu không tồn tại
            if (detail == null)
            {
                _logger.LogWarning("BookingFoodServiceDetail with ID {Id} not found.", detailId);
                return NotFound(new { message = $"BookingFoodServiceDetail with ID {detailId} not found." });
            }

            // Xóa chi tiết
            _context.BookingFoodServiceDetails.Remove(detail);
            _context.SaveChanges();

            _logger.LogInformation("BookingFoodServiceDetail with ID {Id} deleted.", detailId);
            return NoContent();
        }

    }
}
