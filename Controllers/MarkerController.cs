﻿using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq.Expressions;


namespace KartverketRegister.Controllers
{

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
        public IActionResult SubmitMarker([FromBody] Marker marker) // Tar imot en markør via POST-forespørsel
        {
            SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName); // Oppretter en databaseforbindelse
            
            string UserIdString = _userManager.GetUserId(HttpContext?.User);
            int UserId = int.TryParse(UserIdString, out var id) ? id : 0;
            Console.WriteLine($"Cap shit {UserId}");

            try //try-catch for å håndtere feil
            {
                
                seq.SaveMarker(
                    marker.Type,
                    marker.Description,
                    marker.Lat,
                    marker.Lng,
                    userId: UserId,
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
        public IActionResult FetchMyMarkers() // Lage en for admin der User blir ikke blah blah 
        {
            SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);

            string UserIdString = _userManager.GetUserId(HttpContext?.User);
            int UserId = int.TryParse(UserIdString, out var id) ? id : 0;

            try
            {
                List<Marker> MyMarkers = seq.FetchMyMarkers(UserId); // Henter markører for bruker med ID 1
                return Ok(MyMarkers);
            } catch
            {
                return NoContent();
            }
            
        }


    }
}