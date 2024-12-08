namespace Data.Models
{
    public class Invoice
    {
        public required int InvoiceID { get; set; }
        public required int BookingID { get; set; }
        public required int TotalAmount { get; set; }
        public required string PaymentStatus { get; set; } = "Pending";
        public required DateTime PaymentDate { get; set; }
    }
}