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
	}
}
