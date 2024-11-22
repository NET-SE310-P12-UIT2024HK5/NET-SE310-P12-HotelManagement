﻿namespace Hotel_Management.Middleware
{
    public class RoleMiddleWare
    {
        private readonly RequestDelegate _next;

        public RoleMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Gán role tạm thời (có thể lấy từ session hoặc JWT)
            context.Items["Role"] = "Admin";
            //context.Items["Role"] = "Reception";

            await _next(context);
        }
    }
}