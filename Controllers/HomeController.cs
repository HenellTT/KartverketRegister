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


    //viser startsuden eller sender bruker videre basert på rolle
    public async Task<IActionResult> Index()
    {
        AppUser appUser = null;
        try
        {
            appUser = await _userManager.GetUserAsync(HttpContext?.User);
        } catch
        {
            return View();
        }

        // If the user is NOT logged in
        if (appUser == null)
        {
             // Loads the homepage (Index.cshtml)
             return View();
        }

        // User is logged in → redirect based on UserType
        switch (appUser.UserType)
        {
            case "User":
                return RedirectToAction("Index", "Pilot");

            case "Admin":
                return RedirectToAction("Index", "Superadmin");

            case "Employee":
                return RedirectToAction("Index", "Admin");

            default:
                // fallback if UserType is unknown
                return RedirectToAction("Index", "Home");
        }
    }

    //viser brukersiden hvis brukeren er logget inn
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

