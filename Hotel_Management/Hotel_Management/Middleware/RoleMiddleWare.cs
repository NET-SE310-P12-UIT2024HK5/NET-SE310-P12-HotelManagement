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
            string role = null;
            string username = null;

            // Kiểm tra role trong session trước
            var token = context.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var claimsPrincipal = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                    role = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
					username = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
				}
                catch
                {
                    context.Items["Role"] = "Unauthorized";
					context.Items["Username"] = "Unauthorized";
				}
            }

            // Nếu không có trong session, kiểm tra token từ header
            if (string.IsNullOrEmpty(role))
            {
                token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        var claimsPrincipal = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                        role = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
						username = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
					}
                    catch
                    {
                        context.Items["Role"] = "Unauthorized";
                    }
                }
            }

            // Lưu role vào HttpContext.Items để dùng trong các phần khác
            context.Items["Role"] = role ?? "Unauthorized";
			context.Items["Username"] = username ?? "Unauthorized";

			// Tiếp tục pipeline
			await _next(context);
        }
    }
}
