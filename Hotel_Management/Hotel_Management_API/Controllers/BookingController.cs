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

				// Kiểm tra ngày check-in và check-out
				if (bookingDTO.CheckInDate > bookingDTO.CheckOutDate)
				{
					return BadRequest(new { message = "Check-in date must be before check-out date." });
				}

				// Kiểm tra ngày check-in không được là ngày trong quá khứ
				if (bookingDTO.CheckInDate.Date < DateTime.Now.Date)
				{
					return BadRequest(new { message = "Check-in date cannot be in the past." });
				}

				// Kiểm tra xem phòng có được đặt trong khoảng thời gian này không
				var existingBooking = await _context.Booking
					.Where(b => b.RoomID == bookingDTO.RoomID)
					.Where(b => (bookingDTO.CheckInDate < b.CheckOutDate) && (bookingDTO.CheckOutDate > b.CheckInDate))
					.FirstOrDefaultAsync();

				if (existingBooking != null)
				{
					return BadRequest(new
					{
						message = "This room is already booked for the selected dates.",
						conflictBooking = new
						{
							checkIn = existingBooking.CheckInDate,
							checkOut = existingBooking.CheckOutDate
						}
					});
				}

				// Tìm phòng để cập nhật trạng thái
				var room = await _context.Rooms.FindAsync(bookingDTO.RoomID);
				if (room == null)
				{
					return BadRequest(new { message = "Room not found." });
				}

				// Cập nhật trạng thái phòng thành Occupied
				room.Status = "Occupied";

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

				_context.Booking.Add(booking);
				await _context.SaveChangesAsync();

				// Load related data
				var createdBooking = await _context.Booking
					.Include(b => b.Customer)
					.Include(b => b.Room)
					.FirstOrDefaultAsync(b => b.BookingID == booking.BookingID);

				return Ok(new
				{
					message = "Booking created successfully",
					data = createdBooking,
					roomStatus = room.Status
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in CreateBooking");
				return StatusCode(500, new
				{
					message = "An error occurred while creating the booking.",
					details = ex.Message
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

			// Kiểm tra nếu BookingID có tồn tại trong bảng Booking
			bool isBookingLinkedToService = _context.BookingFoodServices.Any(b => b.BookingID == id);
			if (isBookingLinkedToService)
			{
				return Conflict(new { message = "This booking is associated with existing bookings and cannot be deleted." });
			}


			// Nếu không liên kết, thực hiện xóa
			_context.Booking.Remove(booking);
            _context.SaveChanges();

            return Ok(new { message = "Booking deleted successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingDTO bookingDTO)
        {
            try
            {
                if (bookingDTO == null)
                {
                    return BadRequest(new { message = "Booking data is required." });
                }

                // Tìm booking cần chỉnh sửa
                var existingBooking = await _context.Booking.FindAsync(id);

                if (existingBooking == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }

                // Kiểm tra ngày check-in và check-out
                if (bookingDTO.CheckInDate > bookingDTO.CheckOutDate)
                {
                    return BadRequest(new { message = "Check-in date must be before check-out date." });
                }

                // Kiểm tra xem phòng có được đặt trong khoảng thời gian này không
                var conflictBooking = await _context.Booking
                    .Where(b => b.RoomID == bookingDTO.RoomID && b.BookingID != id) // Exclude the current booking
                    .Where(b => (bookingDTO.CheckInDate < b.CheckOutDate) && (bookingDTO.CheckOutDate > b.CheckInDate))
                    .FirstOrDefaultAsync();

                if (conflictBooking != null)
                {
                    return BadRequest(new
                    {
                        message = "This room is already booked for the selected dates.",
                        conflictBooking = new
                        {
                            checkIn = conflictBooking.CheckInDate,
                            checkOut = conflictBooking.CheckOutDate
                        }
                    });
                }

                // Cập nhật thông tin booking
                existingBooking.CustomerID = bookingDTO.CustomerID;
                existingBooking.RoomID = bookingDTO.RoomID;
                existingBooking.CheckInDate = bookingDTO.CheckInDate;
                existingBooking.CheckOutDate = bookingDTO.CheckOutDate;
                existingBooking.Status = bookingDTO.Status;

                _context.Booking.Update(existingBooking);
                await _context.SaveChangesAsync();

                // Load dữ liệu liên kết
                var updatedBooking = await _context.Booking
                    .Include(b => b.Customer)
                    .Include(b => b.Room)
                    .FirstOrDefaultAsync(b => b.BookingID == id);

                return Ok(new { message = "Booking updated successfully", data = updatedBooking });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateBooking");
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the booking.",
                    details = ex.Message
                });
            }
        }

    }
}
