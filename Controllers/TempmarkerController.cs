using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq.Expressions;


namespace KartverketRegister.Controllers
{

    public class TempmarkerController : Controller
    {
        //private int MsgLimit = 15;
        public IActionResult Index()
        {
            return BadRequest("Nothing to see here");
        }

        [HttpPost]
        public IActionResult SubmitMarker()
        {
            
            SequelTempmarker seq = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                string type = Request.Form["type"];
                string description = Request.Form["description"];

                double lat = double.Parse(Request.Form["lat"], CultureInfo.InvariantCulture);
                double lng = double.Parse(Request.Form["lng"], CultureInfo.InvariantCulture);
                decimal height = decimal.Parse(Request.Form["height"], CultureInfo.InvariantCulture);

                seq.SaveMarker(type, description, lat, lng);
                return Ok();
            } catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Potato hihi");
            }
            //return BadRequest('idfk what went wrong at this point');
            

        }
        [HttpGet]
        public IActionResult FetchMyMarkers()
        {
            SequelTempmarker seq = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName);
            List<TempMarker> MyMarkers = seq.FetchMyMarkers(1); // Currently 1 to simulate UserId; 
            return Ok(MyMarkers);
        }
        [HttpGet]
        public IActionResult DeleteMarker(int markerId)
        {
            SequelTempmarker seq = new SequelTempmarker(Constants.DataBaseIp, Constants.DataBaseName);
            seq.DeleteMarkerById(markerId);
            return Ok();
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