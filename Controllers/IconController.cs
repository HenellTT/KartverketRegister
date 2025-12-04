using Microsoft.AspNetCore.Mvc;

namespace KartverketRegister.Controllers
{
    // Returnerer lister over tilgjengelige ikoner og gifs for frontend
    public class IconController : Controller
    {
        [HttpGet]
        public IActionResult Index() => Ok("Icon API");

        // Henter alle ikoner og gifs fra wwwroot/img/-mappene
        [HttpGet]
        public IActionResult GetIcons()
        {
            var iconDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/icons");
            var gifDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/gifs");

            var icons = Directory.GetFiles(iconDir)
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();

            var gifs = Directory.GetFiles(gifDir)
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();

            return Json(new { Icons = icons, Gifs = gifs });
        }
    }
}