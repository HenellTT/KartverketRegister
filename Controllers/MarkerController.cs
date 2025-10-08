using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq.Expressions;


namespace KartverketRegister.Controllers
{

    public class MarkerController : Controller // Arver fra Controller for å håndtere markører
    {
        public IActionResult Index()
        {
            return BadRequest("Nothing to see here"); 
        }

        [HttpPost]
        public IActionResult SubmitMarker(Marker marker) // Tar imot en markør via POST-forespørsel
        {
            SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName); // Oppretter en databaseforbindelse
            try //try-catch for å håndtere feil
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
                
                return Ok(new GeneralResponse(true, "The marker has been registered!"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Ok(new GeneralResponse(false, "The marker could not be registered!"));
            }
            
        }
        [HttpGet]
        public IActionResult FetchMyMarkers()
        {
            SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                List<Marker> MyMarkers = seq.FetchMyMarkers(1); // Henter markører for bruker med ID 1
                return Ok(MyMarkers);
            } catch
            {
                return NoContent();
            }
            
        }


    }
}