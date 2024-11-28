namespace Data.Models
{
    public class Invoice
    {
        public required int InvoiceID { get; set; }
        public required decimal TotalAmount { get; set; }
        public required PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public required DateTime PaymentDate { get; set; }
        public required ICollection<InvoiceBooking> InvoiceBookings { get; set; } = new List<InvoiceBooking>();
    }

    public class InvoiceBooking
    {
        public required int InvoiceID { get; set; }
        public required Invoice Invoice { get; set; }
        public required int BookingID { get; set; }
        public required Booking Booking { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Paid
    }
}