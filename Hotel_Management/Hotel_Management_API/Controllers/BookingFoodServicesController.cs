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
				.FirstOrDefault(s => s.BookingFoodServiceID == id);
			if (service == null) return NotFound();
			return Ok(service);
		}

		[HttpPost]
		public IActionResult Create([FromBody] BookingFoodServices bookingFoodService)
		{
			_context.BookingFoodServices.Add(bookingFoodService);
			_context.SaveChanges();
			return CreatedAtAction(nameof(GetById), new { id = bookingFoodService.BookingFoodServiceID }, bookingFoodService);
		}

		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] BookingFoodServices bookingFoodService)
		{
			var existing = _context.BookingFoodServices.FirstOrDefault(s => s.BookingFoodServiceID == id);
			if (existing == null) return NotFound();

			existing.TotalPrice = bookingFoodService.TotalPrice;
			existing.OrderTime = bookingFoodService.OrderTime;

			_context.SaveChanges();
			return NoContent();
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var existing = _context.BookingFoodServices.FirstOrDefault(s => s.BookingFoodServiceID == id);
			if (existing == null) return NotFound();

			_context.BookingFoodServices.Remove(existing);
			_context.SaveChanges();
			return NoContent();
		}

		[HttpPost]
		public IActionResult CreateBookingFoodService([FromBody] SubmitOrderModel model)
		{
			if (model == null || model.OrderItems == null || !model.OrderItems.Any())
			{
				return BadRequest("Invalid data.");
			}

			try
			{
				// Tạo BookingFoodService
				var bookingFoodService = new BookingFoodServices
				{
					BookingID = model.BookingID,
					TotalPrice = model.OrderItems.Sum(item => item.Quantity * _context.FoodAndBeverageServices
																	 .FirstOrDefault(f => f.ServiceID == item.ServiceID)?.ItemPrice ?? 0),
					OrderTime = DateTime.Now
				};

				_context.BookingFoodServices.Add(bookingFoodService);
				_context.SaveChanges();

				// Tạo BookingFoodServiceDetails
				foreach (var orderItem in model.OrderItems)
				{
					var detail = new BookingFoodServiceDetails
					{
						BookingFoodServiceID = bookingFoodService.BookingFoodServiceID,
						ServiceID = orderItem.ServiceID,
						Quantity = orderItem.Quantity
					};

					_context.BookingFoodServiceDetails.Add(detail);
				}

				_context.SaveChanges();

				return Ok(new { message = "Order created successfully." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while creating booking food service.");
				return StatusCode(500, "An unexpected error occurred.");
			}
		}

	}
}
