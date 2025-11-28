using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KartverketRegister.Controllers
{
    // Håndterer permanente markører (innmeldinger)
    [Authorize(Roles = "User,Employee,Admin")]
    public class MarkerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public MarkerController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() => BadRequest("Nothing to see here");

        // Konverterer TempMarker til permanent Marker
        // TempMarker inneholder koordinater fra kartet, Marker inneholder skjemadata
        [HttpPost]
        public async Task<IActionResult> SubmitMarker([FromBody] Marker marker)
        {
            var seqTemp = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName);
            var seqMarker = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);

            // Hent TempMarker - inneholder Lat/Lng/GeoJson fra kart-tegning
            TempMarker tempMarker = seqTemp.FetchMarkerById(marker.TempMarkerId);

            string userIdString = _userManager.GetUserId(HttpContext?.User);
            int userId = int.TryParse(userIdString, out var id) ? id : 0;
            var appUser = await _userManager.GetUserAsync(HttpContext?.User);

            try
            {
                seqMarker.SaveMarker(
                    marker.Type,
                    marker.Description,
                    tempMarker.Lat,
                    tempMarker.Lng,
                    userId: userId,
                    organization: appUser?.Organization,
                    heightM: marker.HeightM,
                    heightMOverSea: tempMarker.HeightMOverSea,
                    accuracyM: marker.AccuracyM,
                    obstacleCategory: marker.ObstacleCategory,
                    isTemporary: marker.IsTemporary,
                    lighting: marker.Lighting,
                    source: marker.Source,
                    geojson: tempMarker.GeoJson
                );

                return Ok(new GeneralResponse(true, "Marker registered successfully"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Ok(new GeneralResponse(false, "Could not register marker"));
            }
        }

        // Henter alle markører for innlogget bruker
        [HttpGet]
        public IActionResult FetchMyMarkers()
        {
            var seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);

            string userIdString = _userManager.GetUserId(HttpContext?.User);
            int userId = int.TryParse(userIdString, out var id) ? id : 0;

            try
            {
                List<Marker> myMarkers = seq.FetchMyMarkers(userId);
                return Ok(myMarkers);
            }
            catch
            {
                return NoContent();
            }
        }

        // Henter alle godkjente markører for visning på kartet (ingen brukerdata)
        [HttpGet]
        public IActionResult GetObstacles()
        {
            var seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                List<LocationModel> markers = seq.GetObstacles();
                return Ok(new GeneralResponse(true, "Obstacles fetched", markers));
            }
            catch
            {
                return Ok(new GeneralResponse(false, "No obstacles found"));
            }
        }
    }
}
