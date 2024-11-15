using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
