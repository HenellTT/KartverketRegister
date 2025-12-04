namespace KartverketRegister.Models
{
    public class ReviewAssign
    {
        //Modell for tildeling av vurderinger til brukere for en spesifikk markør
        public int UserId { get; set; }
        public int MarkerId { get; set; }
        public ReviewAssign(int userId, int markerId)
        {
            UserId = userId;
            MarkerId = markerId;
        }
    }
}
