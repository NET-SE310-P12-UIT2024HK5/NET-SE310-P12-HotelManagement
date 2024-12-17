using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Hotel_Management_API.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class BookingFoodServiceDetailsController : ControllerBase
	{
		private readonly DatabaseContext _context;
		private readonly ILogger<BookingFoodServiceDetailsController> _logger;

		public BookingFoodServiceDetailsController(DatabaseContext context, ILogger<BookingFoodServiceDetailsController> logger)
		{
			_context = context;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Get()
		{
			var details = _context.BookingFoodServiceDetails.ToList();
			return Ok(details);
		}

		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var detail = _context.BookingFoodServiceDetails
				.FirstOrDefault(d => d.BookingFoodServiceDetailID == id);

			if (detail == null)
			{
				_logger.LogWarning("BookingFoodServiceDetail with ID {Id} not found.", id);
				return NotFound();
			}
			return Ok(detail);
		}

		[HttpPost]
		public IActionResult Create([FromBody] BookingFoodServiceDetails bookingFoodServiceDetail)
		{
			if (bookingFoodServiceDetail == null)
			{
				_logger.LogWarning("Invalid BookingFoodServiceDetail data received.");
				return BadRequest("Invalid data.");
			}

			_context.BookingFoodServiceDetails.Add(bookingFoodServiceDetail);
			_context.SaveChanges();

			_logger.LogInformation("BookingFoodServiceDetail created with ID {Id}", bookingFoodServiceDetail.BookingFoodServiceDetailID);
			return CreatedAtAction(nameof(GetById), new { id = bookingFoodServiceDetail.BookingFoodServiceDetailID }, bookingFoodServiceDetail);
		}

		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] BookingFoodServiceDetails bookingFoodServiceDetail)
		{
			var existing = _context.BookingFoodServiceDetails.FirstOrDefault(d => d.BookingFoodServiceDetailID == id);
			if (existing == null)
			{
				_logger.LogWarning("BookingFoodServiceDetail with ID {Id} not found.", id);
				return NotFound();
			}

			existing.Quantity = bookingFoodServiceDetail.Quantity;
			existing.ServiceID = bookingFoodServiceDetail.ServiceID;

			_context.SaveChanges();
			_logger.LogInformation("BookingFoodServiceDetail with ID {Id} updated.", id);

			return NoContent();
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var existing = _context.BookingFoodServiceDetails.FirstOrDefault(d => d.BookingFoodServiceDetailID == id);
			if (existing == null)
			{
				_logger.LogWarning("BookingFoodServiceDetail with ID {Id} not found.", id);
				return NotFound();
			}

			_context.BookingFoodServiceDetails.Remove(existing);
			_context.SaveChanges();
			_logger.LogInformation("BookingFoodServiceDetail with ID {Id} deleted.", id);

			return NoContent();
		}

		[HttpPost]
		[Route("SubmitOrder")]
		public IActionResult SubmitOrder([FromBody] SubmitOrderModel model)
		{
			if (model == null || model.OrderItems == null || !model.OrderItems.Any())
			{
				_logger.LogWarning("Invalid order data received: {@model}", model);
				return BadRequest("Invalid order data.");
			}

			try
			{
				// Xử lý logic đặt hàng
				var existingBooking = _context.BookingFoodServices
					.FirstOrDefault(bfs => bfs.BookingID == model.BookingID);

				if (existingBooking == null)
				{
					existingBooking = new BookingFoodServices
					{
						BookingID = model.BookingID,
						TotalPrice = 0,
						OrderTime = DateTime.Now
					};

					_context.BookingFoodServices.Add(existingBooking);
					_context.SaveChanges();
				}

				// Cập nhật TotalPrice và thêm chi tiết
				foreach (var orderItem in model.OrderItems)
				{
					var price = _context.FoodAndBeverageServices
						.FirstOrDefault(f => f.ServiceID == orderItem.ServiceID)?.ItemPrice ?? 0;

					existingBooking.TotalPrice += price * orderItem.Quantity;

					var detail = new BookingFoodServiceDetails
					{
						BookingFoodServiceID = existingBooking.BookingFoodServiceID,
						ServiceID = orderItem.ServiceID,
						Quantity = orderItem.Quantity
					};

					_context.BookingFoodServiceDetails.Add(detail);
				}

				_context.SaveChanges();
				return Ok(new { message = "Order processed successfully.", BookingFoodServiceID = existingBooking.BookingFoodServiceID });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while processing order.");
				return StatusCode(500, "An unexpected error occurred.");
			}
		}

        [HttpDelete("DeleteDetail/{detailId}")]
        public IActionResult DeleteDetail(int detailId)
        {
            try
            {
                // Tìm chi tiết cần xóa
                var detail = _context.BookingFoodServiceDetails
                    .Include(d => d.BookingFoodServices)
                    .Include(d => d.FoodAndBeverageService)
                    .FirstOrDefault(d => d.BookingFoodServiceDetailID == detailId);

                if (detail == null)
                {
                    _logger.LogWarning("BookingFoodServiceDetail with ID {Id} not found.", detailId);
                    return NotFound();
                }

                // Tính lại tổng giá
                var priceToSubtract = detail.FoodAndBeverageService.ItemPrice * detail.Quantity;
                var bookingFoodService = detail.BookingFoodServices;
                bookingFoodService.TotalPrice -= priceToSubtract;

                // Xóa chi tiết
                _context.BookingFoodServiceDetails.Remove(detail);
                _context.SaveChanges();

                _logger.LogInformation("BookingFoodServiceDetail with ID {Id} deleted. Updated total price to {TotalPrice}",
                    detailId, bookingFoodService.TotalPrice);

                return Ok(new
                {
                    message = "Chi tiết đã được xóa thành công.",
                    totalPrice = bookingFoodService.TotalPrice,
                    bookingFoodServiceId = bookingFoodService.BookingFoodServiceID
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa chi tiết dịch vụ");
                return StatusCode(500, "Có lỗi xảy ra khi xóa chi tiết.");
            }
        }

    }
}
