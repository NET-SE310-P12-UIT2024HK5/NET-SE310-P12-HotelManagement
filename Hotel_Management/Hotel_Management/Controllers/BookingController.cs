using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers
{
    public class BookingController : Controller
    {
        private readonly ILogger<BookingController> _logger;
        public BookingController(ILogger<BookingController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
