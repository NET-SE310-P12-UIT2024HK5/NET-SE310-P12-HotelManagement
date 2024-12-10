using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Hotel_Management_API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class RoomsController : ControllerBase
    {
		private readonly ILogger<RoomsController> _logger;
		private readonly DatabaseContext _context;

		// Khởi tạo controller với dependency injection
		public RoomsController(ILogger<RoomsController> logger, DatabaseContext context)
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
				var canConnect = await _context.Rooms.FirstOrDefaultAsync();

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

		[HttpGet]
		public IActionResult Get()
		{
			try
			{
				var rooms = _context.Rooms
					.Select(r => new
					{
						r.RoomID,
						r.RoomNumber,
						r.RoomType,
						r.Price,
						r.MaxOccupancy,
						r.Status,
						r.Description
					});
				return Ok(rooms);
			}
			catch (Exception ex)
			{
				// Xử lý lỗi nếu có
				return StatusCode(500, new { message = ex.Message });
			}
		}

        [HttpPost]
        public IActionResult CreateRoom([FromBody] Rooms room)
        {
            try
            {
                if (room == null)
                {
                    return BadRequest(new { message = "Room data is required." });
                }

                // Check for duplicate RoomNumber
                var existingRoom = _context.Rooms.FirstOrDefault(r => r.RoomNumber == room.RoomNumber);
                if (existingRoom != null)
                {
                    return Conflict(new { message = "A room with this number already exists." });
                }

                // Additional validations
                if (string.IsNullOrEmpty(room.RoomNumber) || string.IsNullOrEmpty(room.RoomType))
                {
                    return BadRequest(new { message = "Room number and room type are required." });
                }

                // Add room to the database
                _context.Rooms.Add(room);
                _context.SaveChanges();
                return CreatedAtAction(nameof(Get), new { id = room.RoomID }, room);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { message = "An error occurred while saving the room.", details = dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRoom(int id)
        {
            // Check if the Room exists
            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound(new { message = "Room not found." });
            }

            // Check if the RoomID exists in the Booking table
            bool isRoomLinkedToBooking = _context.Booking.Any(b => b.RoomID == id);
            if (isRoomLinkedToBooking)
            {
                return Conflict(new { message = "This room is associated with existing bookings and cannot be deleted." });
            }

            // If not linked, proceed to delete
            _context.Rooms.Remove(room);
            _context.SaveChanges();

            return Ok(new { message = "Room deleted successfully." });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRoom(int id, [FromBody] Rooms updatedRoom)
        {
            try
            {
                // Check if the room exists
                var existingRoom = _context.Rooms.Find(id);
                if (existingRoom == null)
                {
                    return NotFound(new { message = "Room not found." });
                }

                // Check for duplicate room number with another room
                var duplicateRoom = _context.Rooms
                    .FirstOrDefault(r => r.RoomNumber == updatedRoom.RoomNumber && r.RoomID != id);

                if (duplicateRoom != null)
                {
                    return Conflict(new { message = "A room with this number already exists." });
                }

                // Update room information
                existingRoom.RoomNumber = updatedRoom.RoomNumber;
                existingRoom.RoomType = updatedRoom.RoomType;
                existingRoom.Price = updatedRoom.Price;
                existingRoom.Status = updatedRoom.Status;
                existingRoom.Description = updatedRoom.Description;

                // Save changes
                _context.SaveChanges();

                return Ok(new { message = "Room updated successfully.", room = existingRoom });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the room.", details = ex.Message });
            }
        }
    }
}