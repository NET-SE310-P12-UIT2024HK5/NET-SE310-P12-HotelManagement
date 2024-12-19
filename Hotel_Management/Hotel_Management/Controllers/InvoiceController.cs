using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hotel_Management.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(HttpClient httpClient, ILogger<InvoiceController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Items["Role"]?.ToString();

            if (role == "Admin")
            {
                return RedirectToAction("AdminInvoice");
            }
            else if (role == "Reception")
            {
                return RedirectToAction("ReceptionInvoice");
            }

            return View("Error");
        }

        public async Task<IActionResult> AdminInvoice()
        {
            var invoices = await GetInvoicesAsync();
            var bookings = await GetBookingsAsync();
            if (invoices == null || bookings == null)
            {
                return View("Error");
            }
            ViewBag.Bookings = bookings;
            return View(invoices);
        }

        public async Task<IActionResult> ReceptionInvoice()
        {
            var invoices = await GetInvoicesAsync();
            var bookings = await GetBookingsAsync();
            if (invoices == null || bookings == null)
            {
                return View("Error");
            }
            ViewBag.Bookings = bookings;
            return View(invoices);
        }

        public async Task<List<Invoice>?> GetInvoicesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/Invoice");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error retrieving data from API. Status Code: {StatusCode}, Reason: {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return null;
                }
                var invoices = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Invoice>>(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data from API.");
                return null;
            }
        }

        public async Task<List<Booking>?> GetBookingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/Booking");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error retrieving data from API. Status Code: {StatusCode}, Reason: {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return null;
                }
                var bookings = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Booking>>(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data from API.");
                return null;
            }
        }

        public async Task<int> GetRoomPrice(int bookingId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7287/Invoice/getroomprice/{bookingId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error retrieving room price from API. Status Code: {StatusCode}, Reason: {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return 0;
                }
                var roomPrice = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<int>(roomPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving room price from API.");
                return 0;
            }
        }

        public async Task<int> GetFBTotal(int bookingId)
        {
            try
            {
                _logger.LogInformation("Getting F&B total for Booking ID: {BookingID}", bookingId);
                var response = await _httpClient.GetAsync($"https://localhost:7287/Invoice/fb-total/{bookingId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error retrieving F&B total from API. Status Code: {StatusCode}, Reason: {ReasonPhrase}",
                        response.StatusCode, response.ReasonPhrase);
                    return 0;
                }

                var fbTotal = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<int>(fbTotal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving F&B total from API.");
                return 0;
            }
        }       

        public async Task<IActionResult> CreateInvoice([FromBody] Invoice invoice)
        {
            try
            {
                if (invoice == null)
                {
                    return BadRequest(new { message = "Invalid invoice data." });
                }

                var response = await _httpClient.PostAsJsonAsync("https://localhost:7287/Invoice", invoice);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Invoice created successfully." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating invoice.");
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception while creating invoice.");
                }
                return StatusCode(500, new { message = "An error occurred while creating the invoice.", details = ex.InnerException?.Message });
            }
        }

        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"https://localhost:7287/Invoice/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Invoice deleted successfully." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting invoice.");
                return StatusCode(500, new { message = "An error occurred while deleting the invoice." });
            }
        }

        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] Invoice invoice)
        {
            try
            {
                if (invoice == null)
                {
                    _logger.LogError("Received null invoice data.");
                    return BadRequest(new { message = "Invalid invoice data." });
                }

                _logger.LogInformation("Received invoice data: {@Invoice}", invoice);

                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7287/Invoice/{id}", invoice);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Invoice updated successfully." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating invoice.");
                return StatusCode(500, new { message = "An error occurred while updating the invoice." });
            }
        }
    }
}