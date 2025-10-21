namespace KartverketRegister.Models.Other
{
    public class SpatialReference
    {
        public int wkid { get; set; }
    }

    public class Geometry
    {
        public double x { get; set; }
        public double y { get; set; }
        public SpatialReference spatialReference { get; set; }
    }
}