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
                        u.CCCD,
                        u.DateOfBirth
                    });
                return Ok(customers);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
