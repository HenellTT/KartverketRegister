using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;
using KartverketRegister.Utils;

namespace KartverketRegister.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(); //returnerer viewet Index.cshtml (hjemmesiden)
    }

    public IActionResult Privacy()
    {
        return View(); //returnerer viewet Privacy.cshtml (personvernsiden)
    }

    /*public IActionResult Registry()
    {
        return View(); //returnerer viewet Registry.cshtml (registersiden)
    }*/

    public IActionResult User()
    {
        return View(); //returnerer viewet User.cshtml (brukersiden)
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() //Feilh�ndtering 
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); 
    }

    public IActionResult Registry()
    {
        SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
        try
        {
            List<Marker> myMarkers = seq.FetchMyMarkers(1);
            return View(myMarkers);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View(new List<Marker>());
        }
    }

    public IActionResult EditMarker(int id)
    {
        SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
        Marker marker = seq.FetchMarkerById(id);
        return View(marker);
    }
    
    public IActionResult ViewMarker(int id)
    {
        SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
        Marker marker = seq.FetchMarkerById(id);
        return View(marker);
        
    }

    /*public IActionResult UserView()
    {
        int userId = 1; 

        SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
        List<Marker> userMarkers = seq.FetchMarkersByUserId(userId);
        return View(userMarkers);
    }*/
}
