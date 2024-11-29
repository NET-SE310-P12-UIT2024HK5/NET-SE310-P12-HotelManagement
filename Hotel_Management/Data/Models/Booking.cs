namespace Data.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        // Liên kết đến khách hàng
        public int CustomerID { get; set; }
        public required Customer Customer { get; set; }

        // Liên kết đến phòng
        public int RoomID { get; set; }
        public required Rooms Room { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public required string Status { get; set; }
		public required string CustomerName { get; set; } // Thêm trường này

		public required string RoomNumber { get; set; } // Thêm trường này

    }
}
