namespace Data.Models
{
	public class Booking
	{
		public int BookingID { get; set; }
		public int CustomerID { get; set; }
		public Customer Customer { get; set; }
		public DateTime? CheckInDate { get; set; }
		public DateTime? CheckOutDate { get; set; }
		public string Status { get; set; }
		public string CustomerName { get; set; } // Thêm thuộc tính này

	}
}
