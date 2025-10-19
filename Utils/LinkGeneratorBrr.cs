using KartverketRegister.Models.Other;
using System.Text.Json;
using System.Web;

namespace KartverketRegister.Utils
{
    public static class LinkGeneratorBrr
    {

        public static string HoydeDataCoords(double e, double n) // e = x  -||- n = y
        {
            Geometry gm = new Geometry
            {
                x = e,
                y = n,
                spatialReference = new SpatialReference { wkid = 25833 }
            };

            string jsonGm = JsonSerializer.Serialize(gm);
            string encodedJson = HttpUtility.UrlEncode(jsonGm);

            string url = $"https://hoydedata.no/arcgis/rest/services/NHM_DOM_25833/ImageServer/identify?f=json&geometry={encodedJson}&geometryType=esriGeometryPoint&returnFieldName=false&returnGeometry=false&returnUnformattedValues=false&returnZ=false&layers=top";



            return url;
        }
    }
}
