namespace Data.Models
{
    public class Users
    {
        public required int UserID { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required int RoleID { get; set; }
        public virtual Roles? Roles { get; set; }
    }
}
