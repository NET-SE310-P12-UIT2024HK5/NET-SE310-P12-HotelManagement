using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hotel_Management.Controllers
{
    public class RoomsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RoomsController> _logger;
        public RoomsController(HttpClient httpClient, ILogger<RoomsController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Items["Role"]?.ToString();

            // Kiểm tra role và điều hướng đến view tương ứng
            if (role == "Admin")
            {
                return RedirectToAction("AdminRooms");
            }
            else if (role == "Reception")
            {
                return RedirectToAction("ReceptionRooms");
            }

            return View("Error"); // Nếu role không hợp lệ
        }

        public async Task<IActionResult> AdminRooms()
        {
            var rooms = await GetRoomsAsync();
            if (rooms == null)
            {
                return View("Error");
            }
            return View(rooms);
        }
        public async Task<IActionResult> ReceptionRooms()
        {
            var rooms = await GetRoomsAsync();
            if (rooms == null)
            {
                return View("Error");
            }
            return View(rooms);
        }

        public async Task<List<Rooms>?> GetRoomsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7287/Rooms");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error retrieving data from API.");
                    return null;
                }
                var rooms = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Rooms>>(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data from API.");
                return null;
            }
        }

        public async Task<IActionResult> CreateRoom([FromBody] Rooms room)
        {
            try
            {
                if (room == null)
                {
                    return BadRequest(new { message = "Invalid room data." });
                }

                // Send request to API
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7287/Rooms", room);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Room created successfully." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating room.");
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception while creating room.");
                }
                return StatusCode(500, new { message = "An error occurred while creating the room.", details = ex.InnerException?.Message });
            }
        }

        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                // Send delete request to API
                var response = await _httpClient.DeleteAsync($"https://localhost:7287/Rooms/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Room deleted successfully." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting room.");
                return StatusCode(500, new { message = "An error occurred while deleting the room." });
            }
        }

        public async Task<IActionResult> UpdateRoom(int id, [FromBody] Rooms room)
        {
            try
            {
                if (room == null)
                {
                    _logger.LogError("Received null room data.");
                    return BadRequest(new { message = "Invalid room data." });
                }

                _logger.LogInformation("Received room data: {@Room}", room);

                // Send update request to API
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7287/Rooms/{id}", room);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Room updated successfully." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = errorResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating room.");
                return StatusCode(500, new { message = "An error occurred while updating the room." });
            }
        }

		public async Task<IActionResult> UpdateRoomStatuses()
		{
			try
			{
				var response = await _httpClient.PutAsync("https://localhost:7287/Rooms/update-status", null);

				if (response.IsSuccessStatusCode)
				{
					return Ok(new { message = "Room statuses updated successfully" });
				}
				else
				{
					var errorResponse = await response.Content.ReadAsStringAsync();
					return StatusCode((int)response.StatusCode, new { message = errorResponse });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating room statuses");
				return StatusCode(500, new { message = "An error occurred while updating room statuses" });
			}
		}
	}
}
