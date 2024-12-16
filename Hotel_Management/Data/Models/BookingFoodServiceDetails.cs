namespace Data.Models
{
	public class BookingFoodServiceDetails
	{
		public int BookingFoodServiceDetailID { get; set; }
		public int BookingFoodServiceID { get; set; }
		public int ServiceID { get; set; }
		public int Quantity { get; set; }
		// Navigation properties
		public BookingFoodServices BookingFoodService { get; set; }
		public FoodAndBeverageServices FoodAndBeverageService { get; set; }
	}
}
