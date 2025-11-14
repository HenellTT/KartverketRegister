namespace KartverketRegister.Models
{
    public class AppUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Organization { get; set; } = "";
        public string Email { get; set; } = "";
        public string UserType { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
