namespace KartverketRegister.Models
{
    public class Marker
    {

        public string Type { get; set; }
        public string Description { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public decimal? HeightM { get; set; }
        public decimal? HeightMOverSea { get; set; }
        public string Organization { get; set; }
        public decimal? AccuracyM { get; set; }
        public string ObstacleCategory { get; set; }
        public bool IsTemporary { get; set; }
        public DateTime? ExpectedRemovalDate { get; set; }
        public string Lighting { get; set; }
        public string Source { get; set; }

        public int? UserId { get; set; }
        public int? ReviewedBy { get; set; }
        public string ReviewComment { get; set; }
    }
}
