using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers
{
    public class RoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
