namespace Data.Models
{
	public class SubmitOrderModel
	{
		public int BookingID { get; set; } // Booking liên quan đến đơn hàng
		public List<OrderItem> OrderItems { get; set; } // Danh sách các mục trong đơn hàng

		public class OrderItem
		{
			public int ServiceID { get; set; } // ID của dịch vụ
			public int Quantity { get; set; } // Số lượng dịch vụ
		}
	}
}
