using System.Text.Json.Serialization;

namespace Data.Models
{
	public class BookingFoodServices
	{
		public int BookingFoodServiceID { get; set; }
		public int BookingID { get; set; }
		public int TotalPrice { get; set; }
		public DateTime OrderTime { get; set; }
		// Navigation property
		public Booking Booking { get; set; }

		public ICollection<BookingFoodServiceDetails> BookingFoodServiceDetails { get; set; } = new List<BookingFoodServiceDetails>();
	}
}
