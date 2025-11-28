namespace KartverketRegister.Models.Markers;

/// <summary>
/// Representerer en fullstendig registrert markør/hinder i systemet.
/// Arver grunnleggende egenskaper fra TempMarker.
/// </summary>
public class Marker : TempMarker
{
    public int? MarkerId { get; set; }
    public int TempMarkerId { get; set; }
    
    // Bruker som har gjennomgått markøren
    public new int? UserId { get; set; }
    public int? ReviewedBy { get; set; }
    public string? ReviewComment { get; set; }
    public string? State { get; set; }

    // Høyde og nøyaktighet
    public decimal? HeightM { get; set; }
    public decimal? AccuracyM { get; set; }

    // Hinderkategorisering
    public string? Organization { get; set; }
    public string? ObstacleCategory { get; set; }
    public bool IsTemporary { get; set; }
    public DateTime? ExpectedRemovalDate { get; set; }

    // Tilleggsinfo
    public string? Lighting { get; set; }
    public string? Source { get; set; }
}

