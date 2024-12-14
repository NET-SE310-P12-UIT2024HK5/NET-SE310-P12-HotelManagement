/*namespace Hotel_Management.Middleware
{    public class RoleMiddleWare
    {
        private readonly RequestDelegate _next;

        public RoleMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Gán role tạm thời (có thể lấy từ session hoặc JWT)
            *//*context.Items["Role"] = "Admin";*//*
            context.Items["Role"] = "Reception";
            // Truyền Role sang ViewData để sử dụng trong Razor view
            context.Items["ViewData"] = new Dictionary<string, object>
	        {
		        { "Role", context.Items["Role"] }
	        };
			await _next(context);
        }
    }
}
*/

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Hotel_Management.Middleware
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var claimsPrincipal = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                    var role = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    context.Items["Role"] = role; // Lưu role vào HttpContext
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu token không hợp lệ
                    context.Items["Role"] = "Unauthorized"; // Có thể trả về Unauthorized nếu không parse được token
                }
            }

            // Tiếp tục qua middleware tiếp theo
            await _next(context);
        }
    }
}
