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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() //Feilhåndtering 
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); 
    }
}
