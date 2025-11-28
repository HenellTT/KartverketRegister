using KartverketRegister.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KartverketRegister.Controllers
{
    // Landingsside - redirecter innloggede brukere basert på rolle
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var appUser = await _userManager.GetUserAsync(HttpContext?.User);

            // Ikke innlogget → vis landingsside
            if (appUser == null)
                return View();

            // Innlogget → redirect basert på UserType
            return appUser.UserType switch
            {
                "User" => RedirectToAction("Index", "Pilot"),
                "Admin" => RedirectToAction("Index", "Superadmin"),
                "Employee" => RedirectToAction("Index", "Admin"),
                _ => View()  // Fallback ved ukjent rolle
            };
        }

        [HttpGet]
        public new async Task<IActionResult> User()
        {
            var appUser = await _userManager.GetUserAsync(HttpContext?.User);
            
            if (appUser != null)
                return View("UserLogged", appUser);

            return View();
        }
    }
}
