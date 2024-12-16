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

		public BookingFoodServiceDetailsController(DatabaseContext context)
		{
			_context = context;
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
			if (detail == null) return NotFound();
			return Ok(detail);
		}

		[HttpPost]
		public IActionResult Create([FromBody] BookingFoodServiceDetails bookingFoodServiceDetail)
		{
			_context.BookingFoodServiceDetails.Add(bookingFoodServiceDetail);
			_context.SaveChanges();
			return CreatedAtAction(nameof(GetById), new { id = bookingFoodServiceDetail.BookingFoodServiceDetailID }, bookingFoodServiceDetail);
		}

		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] BookingFoodServiceDetails bookingFoodServiceDetail)
		{
			var existing = _context.BookingFoodServiceDetails.FirstOrDefault(d => d.BookingFoodServiceDetailID == id);
			if (existing == null) return NotFound();

			existing.Quantity = bookingFoodServiceDetail.Quantity;
			existing.ServiceID = bookingFoodServiceDetail.ServiceID;

			_context.SaveChanges();
			return NoContent();
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var existing = _context.BookingFoodServiceDetails.FirstOrDefault(d => d.BookingFoodServiceDetailID == id);
			if (existing == null) return NotFound();

			_context.BookingFoodServiceDetails.Remove(existing);
			_context.SaveChanges();
			return NoContent();
		}
	}
}
