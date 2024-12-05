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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.BookingID);

                entity.HasOne(d => d.Customer)
                    .WithMany()
                    .HasForeignKey(d => d.CustomerID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Room)
                    .WithMany()
                    .HasForeignKey(d => d.RoomID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
