namespace KartverketRegister.Models
{
    public class TempMarker
    {

        // ID for markøren
        public int MarkerId { get; set; }

        //Breddegrad
        public double Lat { get; set; }

        //Lengdegrad
        public double Lng { get; set; }

        //beskrivelse av hinder
        public string Description { get; set; }

        // bruker som registrerer hinder
        public int? UserId { get; set; }

        //type hinder
        public string Type { get; set; }
    }
}
