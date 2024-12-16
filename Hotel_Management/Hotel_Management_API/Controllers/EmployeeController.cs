using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Hotel_Management_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly DatabaseContext _context;

        // Khởi tạo controller với dependency injection
        public EmployeeController(ILogger<EmployeeController> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var employees = _context.Users
                    .Select(u => new
                    {
                        u.UserID,
                        u.FullName,
                        u.PhoneNumber,
                        u.Email,
                        u.Username,
                        u.RoleID
                    });
                return Ok(employees);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Users employee)
        {
            try
            {
                if (employee == null)
                {
                    return BadRequest(new { message = "Employee data is required." });
                }

                // Kiểm tra trùng Username
                var existingEmployee = _context.Users.FirstOrDefault(e => e.Username == employee.Username);
                if (existingEmployee != null)
                {
                    return Conflict(new { message = "An employee with this username already exists." });
                }

                // Các kiểm tra khác
                if (string.IsNullOrEmpty(employee.FullName) || string.IsNullOrEmpty(employee.PhoneNumber))
                {
                    return BadRequest(new { message = "Full name and phone number are required." });
                }

                // Thêm nhân viên vào cơ sở dữ liệu
                _context.Users.Add(employee);
                _context.SaveChanges();
                return CreatedAtAction(nameof(Get), new { id = employee.UserID }, employee);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { message = "An error occurred while saving the employee.", details = dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            // Kiểm tra xem Employee có tồn tại hay không
            var employee = _context.Users.Find(id);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found." });
            }

            // Kiểm tra nếu EmployeeID có tồn tại trong bảng Booking
            bool isEmployeeLinkedToBooking = _context.Booking.Any(b => b.UserID == id);
            if (isEmployeeLinkedToBooking)
            {
                return Conflict(new { message = "This employee is associated with existing bookings and cannot be deleted." });
            }

            // Nếu không liên kết, thực hiện xóa
            _context.Users.Remove(employee);
            _context.SaveChanges();

            return Ok(new { message = "Employee deleted successfully." });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Users updatedEmployee)
        {
            try
            {
                // Kiểm tra xem nhân viên có tồn tại không
                var existingEmployee = _context.Users.Find(id);
                if (existingEmployee == null)
                {
                    return NotFound(new { message = "Employee not found." });
                }

                // Kiểm tra trùng Username với nhân viên khác
                var duplicateEmployee = _context.Users
                    .FirstOrDefault(e => e.Username == updatedEmployee.Username && e.UserID != id);

                if (duplicateEmployee != null)
                {
                    return Conflict(new { message = "An employee with this username already exists." });
                }

                // Cập nhật thông tin nhân viên
                existingEmployee.FullName = updatedEmployee.FullName;
                existingEmployee.PhoneNumber = updatedEmployee.PhoneNumber;
                existingEmployee.Email = updatedEmployee.Email;
                existingEmployee.Username = updatedEmployee.Username;
                existingEmployee.RoleID = updatedEmployee.RoleID;

                // Lưu các thay đổi
                _context.SaveChanges();

                return Ok(new { message = "Employee updated successfully.", employee = existingEmployee });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the employee.", details = ex.Message });
            }
        }
    }
}
