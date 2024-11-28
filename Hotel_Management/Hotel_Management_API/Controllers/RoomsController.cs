using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management_API.Controllers
{
    public class RoomsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
