using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management_API.Controllers
{
    public class SignInController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
