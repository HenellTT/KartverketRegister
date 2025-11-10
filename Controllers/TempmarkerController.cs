using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;


namespace KartverketRegister.Controllers
{
    
    public class TempmarkerController : Controller //arver fra controller for å håndtere midlertidige markører
    {
        private readonly UserManager<AppUser> _userManager;
        public TempmarkerController(

        UserManager<AppUser> userManager
        )
        {
            _userManager = userManager;
        }
        //private int MsgLimit = 15;
        public IActionResult Index()
        {
            return BadRequest("Nothing to see here"); 
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SubmitMarker() //tar imot midlertidig markør via post forespørsel
        {
            
            SequelTempmarker seq = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName); //databaseforbindelse hvor innsendt hinder lagres med data
            try
            {
                string geojson = Request.Form["geojson"];

                int geoJsonBytes = Encoding.UTF8.GetByteCount(geojson);
                int maxBytes = 1000 * 1024; // 1000 KB limit ~1mb

                if (geoJsonBytes > maxBytes)
                {
                    return Json(new GeneralResponse(false, $"GeoJSON too large! Max size is {maxBytes / 1024} KB."));
                }


                string type = Request.Form["type"];
                string description = Request.Form["description"];
                

                double lat = double.Parse(Request.Form["lat"], CultureInfo.InvariantCulture);
                double lng = double.Parse(Request.Form["lng"], CultureInfo.InvariantCulture);
                decimal height = decimal.Parse(Request.Form["height"], CultureInfo.InvariantCulture);

                string UserIdString = _userManager.GetUserId(HttpContext?.User);
                int UserId = int.TryParse(UserIdString, out var id) ? id : 0;

                seq.SaveMarker(type, description, lat, lng, height, UserId, geojson);
                return Json(new GeneralResponse(true, "Marker saved successfully!"));
            } catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return Json(new GeneralResponse(false, "Failed saving marker!")); //lagres ikke data riktig, får man denne feilmeldingen
            }
            
            

        }
        [HttpGet]
        public IActionResult FetchMyMarkers()
        {
            SequelTempmarker seq = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName);
            // Httpcontext.user.userId istd for 1

            string UserIdString = _userManager.GetUserId(HttpContext?.User);
            int UserId = int.TryParse(UserIdString, out var id) ? id : 0;

            List <TempMarker> MyMarkers = seq.FetchMyMarkers(UserId); // Currently 1 to simulate UserId; 
            return Ok(MyMarkers);
        }
        [HttpGet] // Add antiforgery shit, Change to Post 💀
        public IActionResult DeleteMarker(int markerId)
        {
            try
            {
                SequelTempmarker seq = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName);
                string UserIdString = _userManager.GetUserId(HttpContext?.User);
                int UserId = int.TryParse(UserIdString, out var id) ? id : 0;

                GeneralResponse DeleteMarkerResponse = seq.DeleteMarkerById(markerId, UserId);

                return Ok(DeleteMarkerResponse);
            }
            catch (Exception ex) {

                return Ok(new GeneralResponse(false, $"Marker was NOT deleted! {ex}"));
            }
        }
        [HttpGet]
        public IActionResult RegisterMarker(int markerId)
        {

            SequelTempmarker seq = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName);
            TempMarker mrk = seq.FetchMarkerById(markerId);

            return View(mrk);
        }
        
    }
}