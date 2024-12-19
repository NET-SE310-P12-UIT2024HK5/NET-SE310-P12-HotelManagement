using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Data.Models;

namespace Hotel_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7287/";

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = new DashboardViewModel
            {
                RoomStats = await GetRoomStatistics(),
                BookingStats = await GetBookingStatistics(),
                RevenueStats = await GetRevenueStatistics(),
                RecentBookings = await GetRecentBookings()
            };

            return View(dashboardData);
        }

        private async Task<RoomStatistics> GetRoomStatistics()
        {
            try
            {
                var response = await _client.GetAsync("Rooms");
                var content = await response.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<Rooms>>(content);

                return new RoomStatistics
                {
                    TotalRooms = rooms.Count,
                    OccupiedRooms = rooms.Count(r => r.Status == "Occupied"),
                    AvailableRooms = rooms.Count(r => r.Status == "Available"),
                    RoomTypeDistribution = rooms.GroupBy(r => r.RoomType)
                        .Select(g => new KeyValuePair<string, int>(g.Key, g.Count()))
                        .ToDictionary(x => x.Key, x => x.Value)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching room statistics");
                return new RoomStatistics();
            }
        }

        private async Task<BookingStatistics> GetBookingStatistics()
        {
            try
            {
                var response = await _client.GetAsync("Booking");
                var content = await response.Content.ReadAsStringAsync();
                var bookings = JsonConvert.DeserializeObject<List<Booking>>(content);

                var last30Days = DateTime.Now.AddDays(-30);
                var monthlyBookings = bookings
                    .Where(b => b.CheckInDate >= last30Days)
                    .GroupBy(b => b.CheckInDate.Date)
                    .Select(g => new DailyBooking { Date = g.Key, Count = g.Count() })
                    .OrderBy(d => d.Date)
                    .ToList();

                return new BookingStatistics
                {
                    TotalBookings = bookings.Count,
                    ActiveBookings = bookings.Count(b => b.Status == "Active"),
                    MonthlyBookingTrend = monthlyBookings
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching booking statistics");
                return new BookingStatistics();
            }
        }

        private async Task<RevenueStatistics> GetRevenueStatistics()
        {
            try
            {
                var response = await _client.GetAsync("Invoice");
                var content = await response.Content.ReadAsStringAsync();
                var invoices = JsonConvert.DeserializeObject<List<Invoice>>(content);

                var monthlyRevenue = invoices
                    .Where(i => i.PaymentDate >= DateTime.Now.AddMonths(-6))
                    .GroupBy(i => new { Month = i.PaymentDate.Month, Year = i.PaymentDate.Year })
                    .Select(g => new MonthlyRevenue 
                    { 
                        Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                        Amount = g.Sum(i => i.TotalAmount)
                    })
                    .OrderBy(m => m.Month)
                    .ToList();

                return new RevenueStatistics
                {
                    TotalRevenue = invoices.Sum(i => i.TotalAmount),
                    MonthlyRevenueTrend = monthlyRevenue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching revenue statistics");
                return new RevenueStatistics();
            }
        }

        private async Task<List<Booking>> GetRecentBookings()
        {
            try
            {
                var response = await _client.GetAsync("Booking");
                var content = await response.Content.ReadAsStringAsync();
                var bookings = JsonConvert.DeserializeObject<List<Booking>>(content);

                return bookings
                    .OrderByDescending(b => b.CheckInDate)
                    .Take(5)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching recent bookings");
                return new List<Booking>();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}