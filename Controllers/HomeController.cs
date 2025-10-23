using KartverketRegister.Auth;
using KartverketRegister.Models;
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



    [HttpPost]
    public async Task<IActionResult> SetMode(string mode)
    {
        var appUser = await _userManager.GetUserAsync(HttpContext.User);
        ViewBag.Theme = mode ?? "light"; // visning antar lys modus
        return View();
    }
}

