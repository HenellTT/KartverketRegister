using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Threading.Tasks;

namespace KartverketRegister.Controllers
{
    public class ApiController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new GeneralResponse(true,"Api is apiing"));

        }

        [HttpGet]
        // Stjeler MOH value fra HoydeData sin api som en ekte sigma 😎
        public async Task<IActionResult> GetHeight(double lat, double lng)
        {
            var wgs84 = GeographicCoordinateSystem.WGS84;
            var utm33N = ProjectedCoordinateSystem.WGS84_UTM(33, true);

            var transformFactory = new CoordinateTransformationFactory();
            var transform = transformFactory.CreateFromCoordinateSystems(wgs84, utm33N);

            double[] utmCoords = transform.MathTransform.Transform(new double[] { lng, lat });
            double e = utmCoords[0];  // x
            double n = utmCoords[1]; // y
            

            string url = LinkGeneratorBrr.HoydeDataCoords(e, n);

            double? height = await HeightFetcher.GetHeightFromUrlAsync(url);

            return Ok( new { 
                url = url,
                e = e, 
                n = n, 
                lat = lat, 
                lng = lng, 
                height = height
            });
        }
    }
}
