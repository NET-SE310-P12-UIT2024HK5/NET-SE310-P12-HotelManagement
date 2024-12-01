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
        // Phương thức lấy danh sách booking

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                var bookings = await _context.Booking
                                              .Include(b => b.Customer)  // Bao gồm thông tin khách hàng
                                              .Include(b => b.Room)      // Bao gồm thông tin phòng
                                              .ToListAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy danh sách booking: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Lấy danh sách Customer
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _context.Customer.ToListAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách Customer");
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        // Lấy danh sách Room
        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            try
            {
                var rooms = await _context.Rooms.ToListAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách Room");
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDTO bookingDTO)
        {
            try
            {
                if (bookingDTO == null)
                {
                    return BadRequest(new { message = "Booking data is required." });
                }

                // Validate dates
                if (bookingDTO.CheckInDate >= bookingDTO.CheckOutDate)
                {
                    return BadRequest(new { message = "Check-out date must be after check-in date." });
                }

                // Validate IDs
                if (bookingDTO.CustomerID <= 0 || bookingDTO.RoomID <= 0)
                {
                    return BadRequest(new { message = "Invalid Customer ID or Room ID." });
                }

                // Tạo booking mới
                var booking = new Booking
                {
                    CustomerID = bookingDTO.CustomerID,
                    RoomID = bookingDTO.RoomID,
                    CheckInDate = bookingDTO.CheckInDate,
                    CheckOutDate = bookingDTO.CheckOutDate,
                    Status = "Pending",
                    UserID = 1  // Set this to appropriate value
                };

                try
                {
                    _context.Booking.Add(booking);
                    await _context.SaveChangesAsync();

                    // Load related data after successful save
                    var createdBooking = await _context.Booking
                        .Include(b => b.Customer)
                        .Include(b => b.Room)
                        .FirstOrDefaultAsync(b => b.BookingID == booking.BookingID);

                    return Ok(new { message = "Booking created successfully", data = createdBooking });
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database error while creating booking");
                    return StatusCode(500, new
                    {
                        message = "Database error occurred",
                        details = dbEx.InnerException?.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateBooking");
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the booking.",
                    details = ex.Message,
                    stackTrace = ex.StackTrace // Chỉ dùng trong development
                });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBooking(int id)
        {
            // Kiểm tra xem Customer có tồn tại hay không
            var booking = _context.Booking.Find(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found." });
            }

            // Nếu không liên kết, thực hiện xóa
            _context.Booking.Remove(booking);
            _context.SaveChanges();

            return Ok(new { message = "Booking deleted successfully." });
        }

    }
}
