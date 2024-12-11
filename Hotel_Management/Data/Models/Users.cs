namespace Data.Models
{
    public class Users
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleID { get; set; }

        public virtual Roles? Roles { get; set; }
    }
}
