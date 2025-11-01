using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace KartverketRegister.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public HomeController(
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

    public IActionResult Registry()
    {
        return View(); //returnerer viewet Registry.cshtml (registersiden)
    }

    public IActionResult User()
    }
    public async Task<IActionResult> Test()
    {
        var smth = _userManager.GetUserId(HttpContext?.User);
        return Json(smth);
    }
    public async Task<IActionResult> User()
    {

        try
        {
            var appUser = await _userManager.GetUserAsync(HttpContext?.User);
            if (appUser != null)
                
                return View("UserLogged", appUser);
        } catch
        {
            return View(); //returnerer viewet User.cshtml (brukersiden)

        }

        return View();

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

    /*public IActionResult UserView()
    {
        int userId = 1; 

        SequelMarker seq = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
        List<Marker> userMarkers = seq.FetchMarkersByUserId(userId);
        return View(userMarkers);
    }*/
}
