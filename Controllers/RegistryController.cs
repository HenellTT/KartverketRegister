using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Identity.Client;



public class RegistryController : Controller
{
    public IActionResult Registry()
    {
        SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);

        // Hent bruker-ID fra session eller sett til testverdi
        int? userIdFromSession = HttpContext.Session.GetInt32("UserId");
        int userId = userIdFromSession ?? 1; // fallback til testbruker


        List<Marker> myMarkers = seq.FetchMyMarkers(userId);
        Console.WriteLine($"Bruker {userId} har {myMarkers.Count} markører.");

        var model = new RegistryViewModel
        {
            Markers = seq.FetchMyMarkers(userId),
            TempMarkers = new List<Marker>()
        };

        return View(model);
    }

}


