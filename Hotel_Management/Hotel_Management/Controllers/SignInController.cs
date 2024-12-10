using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers
{
    public class SignInController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
