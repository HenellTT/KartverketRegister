using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KartverketRegister.Controllers;
[Authorize(Roles = "User")]

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

    public async Task<IActionResult> User()
    {
        try
        {
            var appUser = await _userManager.GetUserAsync(HttpContext?.User);
            if (appUser != null)

                return View("UserLogged", appUser);
        }
        catch
        {
            return View(); //returnerer viewet User.cshtml (brukersiden)

        }
        return View();
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
    
    [Route("EditMarker/{MarkerId:int}")]
    public IActionResult EditMarker(int MarkerId)
    {
        SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
        Marker marker = seq.FetchMarkerById(MarkerId);

        string UserIdString = _userManager.GetUserId(HttpContext?.User);
        int UserId = int.TryParse(UserIdString, out var id) ? id : 0;

        if (marker.UserId != UserId)
        {
            return Forbid();
        }
        return View(marker);
    }
    [Route("ViewMarker/{MarkerId:int}")]
    public IActionResult ViewMarker(int MarkerId)
    {
        SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
        Marker marker = seq.FetchMarkerById(MarkerId);

        string UserIdString = _userManager.GetUserId(HttpContext?.User);
        int UserId = int.TryParse(UserIdString, out var id) ? id : 0;

        if (marker.UserId != UserId)
        {
            return Forbid();
        }
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

