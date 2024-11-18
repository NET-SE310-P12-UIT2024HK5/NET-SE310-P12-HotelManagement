using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
