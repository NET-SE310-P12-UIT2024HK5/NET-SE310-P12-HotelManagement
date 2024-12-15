using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management_MVC.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username and Password are required.";
                return View();
            }

            // Gọi API để xác thực
            var client = new HttpClient();
            var apiUrl = "https://localhost:7287/api/Account/login"; // Đổi URL API của bạn

            var loginRequest = new
            {
                Username = username,
                Password = password
            };

            var response = await client.PostAsJsonAsync(apiUrl, loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                // Lưu token vào session/cookie nếu cần
                HttpContext.Session.SetString("Token", result.Token);

                // Điều hướng tùy theo Role
                if (result.Role == "Admin")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (result.Role == "Reception")
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Logout()
		{
			// Xóa session
			HttpContext.Session.Clear();

			// Xóa cache của trình duyệt
			Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
			Response.Headers["Pragma"] = "no-cache";
			Response.Headers["Expires"] = "-1";

			// Chuyển hướng về trang đăng nhập
			return RedirectToAction("Login", "Account");
		}

	}

	public class LoginResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
    }
}
