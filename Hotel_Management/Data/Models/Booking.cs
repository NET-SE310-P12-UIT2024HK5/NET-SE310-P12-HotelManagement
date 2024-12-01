namespace Data.Models
{
    public class Booking
    {
        public int BookingID { get; set; } // Khóa chính
        public int CustomerID { get; set; } // Khách hàng đặt phòng
        public int UserID { get; set; } // Nhân viên tiếp nhận đặt phòng
        public int RoomID { get; set; } // Phòng được đặt
        public DateTime CheckInDate { get; set; } // Ngày nhận phòng
        public DateTime CheckOutDate { get; set; } // Ngày trả phòng
        public string Status { get; set; } = "Pending"; // Trạng thái đặt phòng (mặc định là Pending)

        // Navigation Properties
        public virtual Customer Customer { get; set; } // Liên kết với Customer
        public virtual Rooms Room { get; set; } // Liên kết với Room
    }
}
