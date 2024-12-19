using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Hotel_Management_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly ILogger<InvoiceController> _logger;
        private readonly DatabaseContext _context;

        // Khởi tạo controller với dependency injection
        public InvoiceController(ILogger<InvoiceController> logger, DatabaseContext context)
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
                // Kiểm tra kết nối cơ sở dữ liệu bằng cách thử lấy dữ liệu từ bảng Invoice
                var canConnect = await _context.Invoice.FirstOrDefaultAsync();

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
                var invoices = _context.Invoice
                    .Select(i => new
                    {
                        i.InvoiceID,
                        i.BookingID,
                        i.Duration,
                        i.TotalAmount,
                        i.PaymentStatus,
                        i.PaymentDate
                    });
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("getroomprice/{bookingId}")]
        public ActionResult<int> GetRoomPrice(int bookingId)
        {
            var roomPrice = (from booking in _context.Booking
                             join room in _context.Rooms on booking.RoomID equals room.RoomID
                             where booking.BookingID == bookingId
                             select room.Price).FirstOrDefault();

            if (roomPrice == default)
            {
                return NotFound();
            }

            return Ok(roomPrice);
        }

        [HttpGet("fb-total/{bookingId}")]
        public ActionResult<decimal> GetFBTotal(int bookingId)
        {
            try
            {
                var totalPrice = _context.BookingFoodServices
                    .Where(bfs => bfs.BookingID == bookingId)
                    .Select(bfs => bfs.TotalPrice)
                    .FirstOrDefault();

                if (totalPrice == default)
                {
                    return NotFound();
                }

                return Ok(totalPrice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }     

        [HttpPost]
        public IActionResult CreateInvoice([FromBody] Invoice invoice)
        {
            try
            {
                if (invoice == null)
                {
                    return BadRequest(new { message = "Invoice data is required." });
                }

                // Additional validations
                if (invoice.BookingID <= 0 || invoice.TotalAmount <= 0)
                {
                    return BadRequest(new { message = "Booking ID and total amount are required." });
                }                

                // Add invoice to the database
                _context.Invoice.Add(invoice);
                _context.SaveChanges();
                return CreatedAtAction(nameof(Get), new { id = invoice.InvoiceID }, invoice);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { message = "An error occurred while saving the invoice.", details = dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteInvoice(int id)
        {
            // Check if the Invoice exists
            var invoice = _context.Invoice.Find(id);
            if (invoice == null)
            {
                return NotFound(new { message = "Invoice not found." });
            }

            // If not linked, proceed to delete
            _context.Invoice.Remove(invoice);
            _context.SaveChanges();

            return Ok(new { message = "Invoice deleted successfully." });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateInvoice(int id, [FromBody] Invoice updatedInvoice)
        {
            try
            {
                // Check if the invoice exists
                var existingInvoice = _context.Invoice.Find(id);
                if (existingInvoice == null)
                {
                    return NotFound(new { message = "Invoice not found." });
                }

                // Update invoice information
                existingInvoice.BookingID = updatedInvoice.BookingID;
                existingInvoice.Duration = updatedInvoice.Duration;
                existingInvoice.TotalAmount = updatedInvoice.TotalAmount;
                existingInvoice.PaymentStatus = updatedInvoice.PaymentStatus;
                existingInvoice.PaymentDate = updatedInvoice.PaymentDate;

                // Save changes
                _context.SaveChanges();

                return Ok(new { message = "Invoice updated successfully.", invoice = existingInvoice });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the invoice.", details = ex.Message });
            }
        }
    }
}