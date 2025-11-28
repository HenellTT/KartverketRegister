namespace KartverketRegister.Models.Markers;

/// <summary>
/// Enkel lokasjonsmodell for hindre på kartet.
/// </summary>
public class LocationModel
{
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string? ObstacleCategory { get; set; }
    public string? GeoJson { get; set; }
}

