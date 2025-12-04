using System.Data.Common;

namespace KartverketRegister.Models
{
    public class MapMarker : TempMarker
    {
        //representerer en markør på kartet med tilleggsinformasjon
        public string ObstacleCategory { get; set; }
        public string Organization { get; set; }

        // Parameterless constructor
        public MapMarker()
        {
        }

        // Constructor to initialize from a DbDataReader
        internal MapMarker(DbDataReader r)
        {
            MarkerId = r.GetInt32(r.GetOrdinal("MarkerId"));
            Lat = r.GetDouble(r.GetOrdinal("Lat"));
            Lng = r.GetDouble(r.GetOrdinal("Lng"));

            Description = r.IsDBNull(r.GetOrdinal("Description"))
                ? null
                : r.GetString(r.GetOrdinal("Description"));

            UserId = 0;

            Type = r.IsDBNull(r.GetOrdinal("Type"))
                ? null
                : r.GetString(r.GetOrdinal("Type"));

            Organization = r.IsDBNull(r.GetOrdinal("Organization"))
                ? null
                : r.GetString(r.GetOrdinal("Organization"));

            ObstacleCategory = r.IsDBNull(r.GetOrdinal("ObstacleCategory"))
                ? null
                : r.GetString(r.GetOrdinal("ObstacleCategory"));

            HeightMOverSea = r.IsDBNull(r.GetOrdinal("HeightMOverSea"))
                ? (decimal?)null
                : r.GetDecimal(r.GetOrdinal("HeightMOverSea"));
        }
    }
}
