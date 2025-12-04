namespace KartverketRegister.Models
{
    public class Marker : TempMarker
    {
        
        
        //Høydeover havet (kan være null)
        public decimal? HeightM { get; set; }
        
        //Organisasjon som registrerer markøren
        public string Organization { get; set; }
        
        //Nøyaktig i meter (kan være null)
        public decimal? AccuracyM { get; set; }
        
        //kategori for hinder
        public string ObstacleCategory { get; set; }

        //Er hinderet midlertidig?
        public bool IsTemporary { get; set; }

        //forventet fjerningsdato (kan være null)
        public DateTime? ExpectedRemovalDate { get; set; }

        //lysforhold rundt markøren
        public string Lighting { get; set; }

        //kilde for informasjon
        public string Source { get; set; }

        //brukeren som registrerte markøren (kan være null)
        public int? UserId { get; set; }

        //bruker som har gjennomgått markøren (kan være null)
        public int? ReviewedBy { get; set; }

        //kommentar fra review fra admin
        public string ReviewComment { get; set; }

        public string State { get; set; }
        public int? MarkerId { get; set; }

        public int TempMarkerId { get; set; }

    }
}
