namespace Hotel_Management.Models
{
    public class ErrorViewModel
    {
        public string Message { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string RequestId { get; set; }
    }
}
