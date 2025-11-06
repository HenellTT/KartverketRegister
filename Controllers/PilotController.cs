using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;
using KartverketRegister.Utils;

namespace KartverketRegister.Controllers;

public class PilotController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public PilotController(
        ILogger<HomeController> logger,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {
        return View(); //returnerer viewet Index.cshtml (hjemmesiden)
    }

    public IActionResult Privacy()
    {
        return View(); //returnerer viewet Privacy.cshtml (personvernsiden)
    }
    public IActionResult Flightmode()
    {
        return View(); //returnerer viewet Privacy.cshtml (personvernsiden)
    }
    
    public async Task<IActionResult> Test()
    {
        var smth = _userManager.GetUserId(HttpContext?.User);
        return Json(smth);
    }

    public IActionResult FlightMode()
    {
        return View(); //returnerer viewet FlightMode.cshtml (FlyModus)

    }
    
   

    public IActionResult Registry()
    {
        SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
        try
        {
            string UserIdString = _userManager.GetUserId(HttpContext?.User);
            int UserId = int.TryParse(UserIdString, out var id) ? id : 0;

            List<Marker> myMarkers = seq.FetchMyMarkers(UserId);
            return View(myMarkers);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View(new List<Marker>());
        }
    }
    
    [Route("EditMarker/{id:int}")]
    public IActionResult EditMarker(int id)
    {
        SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
        Marker marker = seq.FetchMarkerById(id);
        return View(marker);
    }
    [Route("ViewMarker/{id:int}")]
    public IActionResult ViewMarker(int id)
    {
        SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
        Marker marker = seq.FetchMarkerById(id);
        return View(marker);
    }




    [HttpPost]
    public async Task<IActionResult> SetMode(string mode)
    {
        var appUser = await _userManager.GetUserAsync(HttpContext.User);
        ViewBag.Theme = mode ?? "light"; // visning antar lys modus
        return View();
    }
}

