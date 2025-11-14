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
    public IActionResult Pilot()
    {
        return View(); //returnerer viewet Privacy.cshtml (personvernsiden)
    }
    public IActionResult Admin()
    {
        return View(); //returnerer viewet Privacy.cshtml (personvernsiden)
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
    
    
   

    
    
    
}

