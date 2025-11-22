using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime;


namespace KartverketRegister.Controllers
{
    [Authorize(Roles = "User,Employee,Admin")]

    public class MarkerController : Controller // Arver fra Controller for å håndtere markører
    {
        private readonly UserManager<AppUser> _userManager;
        public MarkerController(

        UserManager<AppUser> userManager
        )
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return BadRequest("Nothing to see here"); 
        }

        [HttpPost]
        public async Task<IActionResult> SubmitMarker([FromBody] Marker marker) // Tar imot en markør via POST-forespørsel
        {
            SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName); // Oppretter en databaseforbindelse
            
            string UserIdString = _userManager.GetUserId(HttpContext?.User);
            int UserId = int.TryParse(UserIdString, out var id) ? id : 0;
            var appUser = await _userManager.GetUserAsync(HttpContext?.User);

            Console.WriteLine($"Cap shit {UserId}");
            Console.WriteLine($"User org {appUser.Organization}");


            try //try-catch for å håndtere feil
            {

                seq.SaveMarker(
                    marker.Type,
                    marker.Description,
                    marker.Lat,
                    marker.Lng,
                    userId: UserId,
                    organization: appUser.Organization,
                    heightM: marker.HeightM,
                    heightMOverSea: marker.HeightMOverSea,
                    accuracyM: marker.AccuracyM,
                    obstacleCategory: marker.ObstacleCategory,
                    isTemporary: marker.IsTemporary,
                    lighting: marker.Lighting,
                    source: marker.Source,
                    geojson: marker.GeoJson
                );
                
                return Ok(new GeneralResponse(true, "The marker has been registered!"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Ok(new GeneralResponse(false, "The marker could not be registered!"));
            }
            
        }
        [HttpGet]
        public IActionResult FetchMyMarkers() // Lage en for admin der User blir ikke blah blah 
        {
            SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);

            string UserIdString = _userManager.GetUserId(HttpContext?.User);
            int UserId = int.TryParse(UserIdString, out var id) ? id : 0;

            try
            {
                List<Marker> MyMarkers = seq.FetchMyMarkers(UserId); // Henter markører for bruker med ID 
                return Ok(MyMarkers);
            } catch
            {
                return NoContent();
            }
            
        }
        // fetch all stripped markers (for view on map, no user data)

        public IActionResult GetObstacles()
        {
            SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                List<LocationModel> Markers = seq.GetObstacles(); 
                return Ok(new GeneralResponse(true, "Here yo markers man", Markers));
            }
            catch
            {
                return Ok(new GeneralResponse(false, "no markers"));
            }
        }


    }
}