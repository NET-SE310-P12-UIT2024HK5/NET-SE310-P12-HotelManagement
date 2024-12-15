using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseContext _context;

        public AccountController(IConfiguration configuration, DatabaseContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and Password are required.");
            }

            // Kiểm tra User từ database
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);

            if (user == null || user.Password != request.Password) // So sánh mật khẩu
            {
                return Unauthorized("Invalid username or password.");
            }

            // Tạo token
            var token = GenerateJwtToken(user);

            return Ok(new LoginResponse
            {
                Token = token,
                Role = _context.Roles.FirstOrDefault(r => r.RoleID == user.RoleID)?.RoleName,
                FullName = user.FullName
            });


        }

        private string GenerateJwtToken(Users user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, _context.Roles.FirstOrDefault(r => r.RoleID == user.RoleID)?.RoleName)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
