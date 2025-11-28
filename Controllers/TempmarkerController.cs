using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace KartverketRegister.Controllers
{
    // Midlertidige markører (utkast) før endelig innsending
    [Authorize(Roles = "User,Employee,Admin")]
    public class TempmarkerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public TempmarkerController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() => BadRequest("Nothing to see here");

        // Lagrer midlertidig markør fra kart-tegning
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SubmitMarker()
        {
            var seq = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName);

            try
            {
                string geojson = Request.Form["geojson"];

                // Begrens GeoJSON-størrelse til ~1MB
                int geoJsonBytes = Encoding.UTF8.GetByteCount(geojson);
                int maxBytes = 1000 * 1024;

                if (geoJsonBytes > maxBytes)
                    return Json(new GeneralResponse(false, $"GeoJSON too large. Max size: {maxBytes / 1024} KB"));

                string type = Request.Form["type"];
                string description = Request.Form["description"];
                double lat = double.Parse(Request.Form["lat"], CultureInfo.InvariantCulture);
                double lng = double.Parse(Request.Form["lng"], CultureInfo.InvariantCulture);
                decimal height = decimal.Parse(Request.Form["height"], CultureInfo.InvariantCulture);

                string userIdString = _userManager.GetUserId(HttpContext?.User);
                int userId = int.TryParse(userIdString, out var id) ? id : 0;

                seq.SaveMarker(type, description, lat, lng, height, userId, geojson);
                return Json(new GeneralResponse(true, "Marker saved successfully"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return Json(new GeneralResponse(false, "Failed to save marker"));
            }
        }

        [HttpGet]
        public IActionResult FetchMyMarkers()
        {
            var seq = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName);

            string userIdString = _userManager.GetUserId(HttpContext?.User);
            int userId = int.TryParse(userIdString, out var id) ? id : 0;

            List<TempMarker> myMarkers = seq.FetchMyMarkers(userId);
            return Ok(myMarkers);
        }

        // Sletter midlertidig markør - database-metoden sjekker eierskap
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult DeleteMarker(int markerId)
        {
            try
            {
                var seq = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName);

                string userIdString = _userManager.GetUserId(HttpContext?.User);
                int userId = int.TryParse(userIdString, out var id) ? id : 0;

                GeneralResponse response = seq.DeleteMarkerById(markerId, userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new GeneralResponse(false, $"Could not delete marker: {ex.Message}"));
            }
        }

        // Viser registreringsskjema for midlertidig markør
        [HttpGet]
        public IActionResult RegisterMarker(int markerId)
        {
            var seq = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName);
            TempMarker marker = seq.FetchMarkerById(markerId);

            string userIdString = _userManager.GetUserId(HttpContext?.User);
            int userId = int.TryParse(userIdString, out var id) ? id : 0;

            // Sjekk at bruker eier markøren
            if (marker.UserId != userId)
                return Forbid();

            return View(marker);
        }
    }
}
