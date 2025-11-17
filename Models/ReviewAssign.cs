namespace KartverketRegister.Models
{
    public class ReviewAssign
    {
        public int UserId { get; set; }
        public int MarkerId { get; set; }
        public ReviewAssign(int userId, int markerId)
        {
            UserId = userId;
            MarkerId = markerId;
        }
    }
}
