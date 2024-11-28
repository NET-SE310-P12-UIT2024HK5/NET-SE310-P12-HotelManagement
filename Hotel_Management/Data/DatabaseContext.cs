using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Data
{
    public class DatabaseContext : DbContext
    {
        // Khai báo DbSet cho các bảng (tables) trong cơ sở dữ liệu
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Rooms> Rooms { get; set; }
        // Bạn có thể khai báo thêm DbSet cho các bảng khác nếu cần

        // Khởi tạo DatabaseContext với DbContextOptions (dùng trong Dependency Injection)
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        // Cấu hình thêm các quy tắc (nếu cần)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình cho các bảng, ví dụ: tên bảng, khoá chính, khoá ngoại, v.v.
            base.OnModelCreating(modelBuilder);
        }
    }
}
