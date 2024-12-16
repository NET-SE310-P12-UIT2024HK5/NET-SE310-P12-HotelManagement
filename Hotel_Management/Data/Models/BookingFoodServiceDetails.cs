using System.Text.Json.Serialization;

namespace Data.Models
{
	public class BookingFoodServiceDetails
	{
		public int BookingFoodServiceDetailID { get; set; }
		public int BookingFoodServiceID { get; set; }
		public int ServiceID { get; set; }
		public int Quantity { get; set; }
		// Navigation properties
		[JsonIgnore]
		public BookingFoodServices BookingFoodServices { get; set; }
		public FoodAndBeverageServices FoodAndBeverageService { get; set; }
	}
}
