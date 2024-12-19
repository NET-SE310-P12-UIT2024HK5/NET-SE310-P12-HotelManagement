namespace Data.Models
{
	public class DashboardViewModel
	{
		public RoomStatistics RoomStats { get; set; }
		public BookingStatistics BookingStats { get; set; }
		public RevenueStatistics RevenueStats { get; set; }
		public List<Booking> RecentBookings { get; set; }
	}

	public class RoomStatistics
	{
		public int TotalRooms { get; set; }
		public int OccupiedRooms { get; set; }
		public int AvailableRooms { get; set; }
		public Dictionary<string, int> RoomTypeDistribution { get; set; }
	}

	public class BookingStatistics
	{
		public int TotalBookings { get; set; }
		public int ActiveBookings { get; set; }
		public List<DailyBooking> MonthlyBookingTrend { get; set; }
	}

	public class DailyBooking
	{
		public DateTime Date { get; set; }
		public int Count { get; set; }
	}

	public class RevenueStatistics
	{
		public decimal TotalRevenue { get; set; }
		public List<MonthlyRevenue> MonthlyRevenueTrend { get; set; }
	}

	public class MonthlyRevenue
	{
		public DateTime Month { get; set; }
		public decimal Amount { get; set; }
	}
}
