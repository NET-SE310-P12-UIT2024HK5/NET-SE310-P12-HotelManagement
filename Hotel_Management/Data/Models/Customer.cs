namespace Data.Models
{
	public class Customer
	{
		public int CustomerID { get; set; }
		public string FullName { get; set; }
		public string PhoneNumber { get; set; }
		public string CCCD { get; set; }
		public string Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		// Quan hệ 1-N với Booking
		/*public ICollection<Booking> Bookings { get; set; }*/
	}
}
