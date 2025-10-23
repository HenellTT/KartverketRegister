using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;



public class RegistryController : Controller
{
    public IActionResult Index()
    {

        string dbIP = Constants.DatabasePath;
        string dbname = Constants.DatabaseName;

        string userIdStr = User.Identity.GetUserId();
        int userId = int.Parse(userIdStr); // ← Konverterer string til int

        SequelMarker max = new SequelMarker(dbIP, dbname);

        List<Marker> maxMarkers = max.FetchMyMarkers(userId);
        Console.WriteLine("MyMarker count: " + maxMarkers.Count);

        var model = new RegistryViewModel
        {
            Markers = maxMarkers,
            TempMarkers = new List<Marker>()
        };

        return View(model);

    }
    public IActionResult Registry()
    {
        SequelMarker seq = new SequelMarker(Constants.DatabasePath;, Constants.DatabaseName;);

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


