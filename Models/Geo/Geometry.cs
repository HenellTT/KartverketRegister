using System.Text.Json.Serialization;

namespace KartverketRegister.Models.Geo;

/// <summary>
/// Spatial reference for koordinatsystem.
/// </summary>
public class SpatialReference
{
    [JsonPropertyName("wkid")]
    public int Wkid { get; set; }
}

/// <summary>
/// Geometri-punkt med koordinater og referansesystem.
/// </summary>
public class Geometry
{
    [JsonPropertyName("x")]
    public double X { get; set; }
    
    [JsonPropertyName("y")]
    public double Y { get; set; }
    
    [JsonPropertyName("spatialReference")]
    public SpatialReference? SpatialReference { get; set; }
}

