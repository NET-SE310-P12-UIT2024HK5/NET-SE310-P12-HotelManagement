using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Rooms
    {
        [Key]
        public required int RoomID { get; set; }
        public  string RoomNumber { get; set; }
        public required string RoomType { get; set; }
        public required decimal Price { get; set; }
        public int MaxOccupancy { get; set; } = 2;
        public string Status { get; set; } = "Available";
        public string? Description { get; set; }
    }
}