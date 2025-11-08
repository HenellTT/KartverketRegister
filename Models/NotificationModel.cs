namespace KartverketRegister.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public int UserId {get; set;}
        public string Message { get; set;}
        public bool IsRead { get; set;}
        public string Type { get; set;}
        public DateTime? Date { get; set;}
    }
}
