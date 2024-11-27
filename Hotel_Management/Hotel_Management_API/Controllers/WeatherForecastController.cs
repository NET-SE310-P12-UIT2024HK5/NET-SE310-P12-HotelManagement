using Microsoft.AspNetCore.Mvc;
using Data.Models;
using Data;
using System.Data.SqlClient;

namespace Hotel_Management_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly DatabaseContext _context;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastController(ILogger<WeatherForecastController> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Phương thức để kiểm tra kết nối đến cơ sở dữ liệu
        [HttpGet("check-connection")]
        public IActionResult CheckDatabaseConnection()
        {
            try
            {
                using (var connection = _context.GetConnection())
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT 1", connection);
                    var result = command.ExecuteScalar();

                    if (result != null && (int)result == 1)
                    {
                        return Ok("Kết nối cơ sở dữ liệu thành công!");
                    }
                    else
                    {
                        return StatusCode(500, "Không thể thực hiện truy vấn tới cơ sở dữ liệu.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kết nối cơ sở dữ liệu");
                return StatusCode(500, $"Lỗi kết nối cơ sở dữ liệu: {ex.Message}");
            }
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
