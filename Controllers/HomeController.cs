using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;

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

    public IActionResult Registry()
    {
        return View(); //returnerer viewet Registry.cshtml (registersiden)
    }

    public IActionResult User()
    {
        return View(); //returnerer viewet User.cshtml (brukersiden)
    }
    public IActionResult FlightMode()
    {
        return View(); //returnerer viewet FlightMode.cshtml (FlyModus)
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() //Feilh�ndtering 
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); 
    }
}
