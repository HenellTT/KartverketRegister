using System.Data.Common;

namespace KartverketRegister.Models.Markers;

/// <summary>
/// Representerer en midlertidig markør/hinder som ikke er fullstendig registrert ennå.
/// </summary>
public class TempMarker
{
    public int MarkerId { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string? Description { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Type { get; set; }
    public decimal? HeightMOverSea { get; set; }
    public string? GeoJson { get; set; }
    public DateTime? Date { get; set; }

    public TempMarker() { }

    internal TempMarker(DbDataReader r)
    {
        MarkerId = r.GetInt32(r.GetOrdinal("MarkerId"));
        Lat = r.GetDouble(r.GetOrdinal("Lat"));
        Lng = r.GetDouble(r.GetOrdinal("Lng"));

        Description = r.IsDBNull(r.GetOrdinal("Description"))
            ? null
            : r.GetString(r.GetOrdinal("Description"));

        UserId = r.IsDBNull(r.GetOrdinal("UserId"))
            ? null
            : r.GetInt32(r.GetOrdinal("UserId"));

        Type = r.IsDBNull(r.GetOrdinal("Type"))
            ? null
            : r.GetString(r.GetOrdinal("Type"));

        HeightMOverSea = r.IsDBNull(r.GetOrdinal("HeightMOverSea"))
            ? null
            : r.GetDecimal(r.GetOrdinal("HeightMOverSea"));

        GeoJson = r.IsDBNull(r.GetOrdinal("GeoJson"))
            ? null
            : r.GetString(r.GetOrdinal("GeoJson"));
    }
}

