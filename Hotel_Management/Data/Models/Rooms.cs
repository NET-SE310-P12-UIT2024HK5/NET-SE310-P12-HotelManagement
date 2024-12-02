using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Rooms
    {
        [Key]
        public required int RoomID { get; set; }
        public required string RoomNumber { get; set; }
        public required string RoomType { get; set; }
        public required int Price { get; set; }
        public int MaxOccupancy
        {
            get
            {
                return RoomType switch
                {
                    "Single" => 1,
                    "Double" => 2,
                    "King" or "Queen" => 4,
                    "Suite" => 8,
                    _ => 2 // Default value if RoomType is not recognized
                };
            }
        }
        public string Status { get; set; } = "Available";
        public string? Description { get; set; }
    }
}