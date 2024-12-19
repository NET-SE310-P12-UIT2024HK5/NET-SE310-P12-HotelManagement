using Hotel_Management.Middleware;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Thêm HttpClient để gọi API
builder.Services.AddHttpClient();

// Thêm Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Thêm Distributed Memory Cache và Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // Thời gian hết hạn của session
    options.Cookie.HttpOnly = true; // Cookie chỉ sử dụng qua HTTP, không dùng qua JS
    options.Cookie.IsEssential = true; // Đảm bảo cookie không bị chặn
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Bật Cors
app.UseCors("AllowSpecificOrigin");

// Sử dụng Session
app.UseSession();

// Middleware phân quyền dựa trên JWT
app.UseMiddleware<RoleMiddleware>();

// Authorization (sau middleware để phân quyền chính xác)
app.UseAuthorization();

// Map routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
