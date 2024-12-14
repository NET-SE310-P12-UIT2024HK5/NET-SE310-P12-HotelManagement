using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hotel_Management.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AccountController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        // GET: Login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login page
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userService.ValidateUserAsync(username, password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            var token = GenerateJwtToken(user);

            // Thêm token vào response headers hoặc lưu trữ trong cookie
            Response.Cookies.Append("JWT", token);

            return RedirectToAction("Index", "Home"); // Điều hướng đến trang chính sau khi đăng nhập
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role) // Role của người dùng
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
