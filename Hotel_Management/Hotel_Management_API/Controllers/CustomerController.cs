using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Hotel_Management_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly DatabaseContext _context;

        // Khởi tạo controller với dependency injection
        public CustomerController(ILogger<CustomerController> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var customers = _context.Customer
                    .Select(u => new
                    {
                        u.CustomerID,
                        u.FullName,
                        u.PhoneNumber,
                        u.Email,
                        u.CCCD
                    });
                return Ok(customers);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, new { message = ex.Message });
            }
        }

		[HttpPost]
		public IActionResult CreateCustomer([FromBody] Customer customer)
		{
			try
			{
				if (customer == null)
				{
					return BadRequest(new { message = "Customer data is required." });
				}

				// Kiểm tra trùng CCCD
				var existingCustomer = _context.Customer.FirstOrDefault(c => c.CCCD == customer.CCCD);
				if (existingCustomer != null)
				{
					return Conflict(new { message = "A customer with this National ID already exists." });
				}

				// Các kiểm tra khác
				if (string.IsNullOrEmpty(customer.FullName) || string.IsNullOrEmpty(customer.PhoneNumber))
				{
					return BadRequest(new { message = "Full name and phone number are required." });
				}

				// Thêm khách hàng vào cơ sở dữ liệu
				_context.Customer.Add(customer);
				_context.SaveChanges();
				return CreatedAtAction(nameof(Get), new { id = customer.CustomerID }, customer);
			}
			catch (DbUpdateException dbEx)
			{
				return StatusCode(500, new { message = "An error occurred while saving the customer.", details = dbEx.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
			}
		}
		[HttpDelete("{id}")]
		public IActionResult DeleteCustomer(int id)
		{
			// Kiểm tra xem Customer có tồn tại hay không
			var customer = _context.Customer.Find(id);
			if (customer == null)
			{
				return NotFound(new { message = "Customer not found." });
			}

			// Kiểm tra nếu CustomerID có tồn tại trong bảng Booking
			bool isCustomerLinkedToBooking = _context.Booking.Any(b => b.CustomerID == id);
			if (isCustomerLinkedToBooking)
			{
				return Conflict(new { message = "This customer is associated with existing bookings and cannot be deleted." });
			}

			// Nếu không liên kết, thực hiện xóa
			_context.Customer.Remove(customer);
			_context.SaveChanges();

			return Ok(new { message = "Customer deleted successfully." });
		}


		[HttpPut("{id}")]
		public IActionResult UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
		{
			try
			{
				// Kiểm tra xem khách hàng có tồn tại không
				var existingCustomer = _context.Customer.Find(id);
				if (existingCustomer == null)
				{
					return NotFound(new { message = "Customer not found." });
				}

				// Kiểm tra trùng CCCD với khách hàng khác
				var duplicateCustomer = _context.Customer
					.FirstOrDefault(c => c.CCCD == updatedCustomer.CCCD && c.CustomerID != id);

				if (duplicateCustomer != null)
				{
					return Conflict(new { message = "A customer with this ID number already exists." });
				}


				// Cập nhật thông tin khách hàng
				existingCustomer.FullName = updatedCustomer.FullName;
				existingCustomer.PhoneNumber = updatedCustomer.PhoneNumber;
				existingCustomer.Email = updatedCustomer.Email;
				existingCustomer.CCCD = updatedCustomer.CCCD;

				// Lưu các thay đổi
				_context.SaveChanges();

				return Ok(new { message = "Customer updated successfully.", customer = existingCustomer });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while updating the customer.", details = ex.Message });
			}
		}

	}
}
