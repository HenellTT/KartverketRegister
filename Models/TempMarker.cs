using System.Data.Common;

namespace KartverketRegister.Models
{
    public class TempMarker
    {
        // ID for markøren
        public int MarkerId { get; set; }

        // Breddegrad
        public double Lat { get; set; }

        // Lengdegrad
        public double Lng { get; set; }

        // Beskrivelse av hinder
        public string Description { get; set; }

        // Bruker som registrerer hinder
        public int? UserId { get; set; }

        public string UserName { get; set; }

        // Type hinder
        public string Type { get; set; }

        public decimal? HeightMOverSea { get; set; }

        public string GeoJson { get; set; }

        // Parameterless constructor – allows empty instance
        public TempMarker()
        {
        }

        // Constructor from DbDataReader
        internal TempMarker(DbDataReader r)
        {
            MarkerId = r.GetInt32(r.GetOrdinal("MarkerId"));
            Lat = r.GetDouble(r.GetOrdinal("Lat"));
            Lng = r.GetDouble(r.GetOrdinal("Lng"));

            Description = r.IsDBNull(r.GetOrdinal("Description"))
                ? null
                : r.GetString(r.GetOrdinal("Description"));

            UserId = r.IsDBNull(r.GetOrdinal("UserId"))
                ? (int?)null
                : r.GetInt32(r.GetOrdinal("UserId"));

            Type = r.IsDBNull(r.GetOrdinal("Type"))
                ? null
                : r.GetString(r.GetOrdinal("Type"));

            HeightMOverSea = r.IsDBNull(r.GetOrdinal("HeightMOverSea"))
                ? (decimal?)null
                : r.GetDecimal(r.GetOrdinal("HeightMOverSea"));


        }
    }
}
