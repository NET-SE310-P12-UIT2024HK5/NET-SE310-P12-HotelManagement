using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Rooms
    {
        [Key]
        public required int RoomID { get; set; }
        public required string RoomNumber { get; set; }
        public required string RoomType { get; set; }
        public required decimal Price { get; set; }
        public int MaxOccupancy { get; set; } = 2;
        public RoomStatus Status { get; set; } = RoomStatus.Available;
        public string? Description { get; set; }
    }

    public enum RoomStatus
    {
        Available,
        Occupied,
        UnderMaintenance
    }
}