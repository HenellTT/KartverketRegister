using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq.Expressions;


namespace KartverketRegister.Controllers
{

    public class MarkerController : Controller
    {
        public IActionResult Index()
        {
            return BadRequest("Nothing to see here");
        }

        [HttpPost]
        public IActionResult SubmitMarker(Marker marker)
        {

            SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                seq.SaveMarker(
                    marker.Type,
                    marker.Description,
                    marker.Lat,
                    marker.Lng,
                    userId: 1,
                    organization: marker.Organization,
                    heightM: marker.HeightM,
                    heightMOverSea: marker.HeightMOverSea,
                    accuracyM: marker.AccuracyM,
                    obstacleCategory: marker.ObstacleCategory,
                    isTemporary: marker.IsTemporary,
                    lighting: marker.Lighting,
                    source: marker.Source
                );
                return Ok("Good good, registered");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Potato hihi");
            }
            //return BadRequest('idfk what went wrong at this point');
        }
        [HttpGet]
        public IActionResult FetchMyMarkers()
        {
            SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                List<Marker> MyMarkers = seq.FetchMyMarkers(1); // Currently 1 to simulate UserId; 
                return Ok(MyMarkers);
            } catch
            {
                return NoContent();
            }
            
        }


    }
}