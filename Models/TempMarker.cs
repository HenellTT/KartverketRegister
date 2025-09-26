namespace KartverketRegister.Models
{
    public class TempMarker
    {
        public int MarkerId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
        public string Type { get; set; }
    }
}
