using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KartverketRegister.Controllers
{
    // Pilot-dashboard for vanlige brukere
    [Authorize(Roles = "User")]
    public class PilotController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public PilotController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public new async Task<IActionResult> User()
        {
            var appUser = await _userManager.GetUserAsync(HttpContext?.User);
            
            if (appUser != null)
                return View("UserLogged", appUser);

            return View();
        }

        [HttpGet]
        public IActionResult FlightMode() => View();

        // Viser brukerens innmeldte markører
        [HttpGet]
        public IActionResult Registry()
        {
            var seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);

            string userIdString = _userManager.GetUserId(HttpContext?.User);
            int userId = int.TryParse(userIdString, out var id) ? id : 0;

            try
            {
                List<Marker> myMarkers = seq.FetchMyMarkers(userId);
                return View(myMarkers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View(new List<Marker>());
            }
        }

        // Redigering av markør - sjekker at bruker eier markøren
        [HttpGet]
        [Route("EditMarker/{markerId:int}")]
        public IActionResult EditMarker(int markerId)
        {
            Marker? marker = FetchMarkerIfOwner(markerId);
            
            if (marker == null)
                return Forbid();

            return View(marker);
        }

        // Visning av markør - sjekker at bruker eier markøren
        [HttpGet]
        [Route("ViewMarker/{markerId:int}")]
        public IActionResult ViewMarker(int markerId)
        {
            Marker? marker = FetchMarkerIfOwner(markerId);
            
            if (marker == null)
                return Forbid();

            return View(marker);
        }

        [HttpPost]
        public IActionResult SetMode(string mode)
        {
            ViewBag.Theme = mode ?? "light";
            return View();
        }

        // Hjelpemetode: Henter markør kun hvis innlogget bruker eier den
        private Marker? FetchMarkerIfOwner(int markerId)
        {
            var seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            Marker marker = seq.FetchMarkerById(markerId);

            string userIdString = _userManager.GetUserId(HttpContext?.User);
            int userId = int.TryParse(userIdString, out var id) ? id : 0;

            if (marker?.UserId != userId)
                return null;

            return marker;
        }
    }
}
