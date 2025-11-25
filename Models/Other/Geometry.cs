namespace KartverketRegister.Models.Other
{
    //Modell for geometriske data
    public class SpatialReference
    {
        public int wkid { get; set; }
    }

    public class Geometry
    {
        //punktsgeometri med x- og y-koordinater for geojson
        public double x { get; set; }
        public double y { get; set; }
        public SpatialReference spatialReference { get; set; }
    }
}