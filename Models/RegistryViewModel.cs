using System.Collections.Generic;

namespace KartverketRegister.Models
{
    public class RegistryViewModel
    {
            public List<Marker> Markers { get; set; } = new List<Marker>();
            public List<Marker> TempMarkers { get; set; } = new List<Marker>();

    }
}

