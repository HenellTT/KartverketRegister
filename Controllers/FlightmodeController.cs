using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KartverketRegister.Controllers
{
    // Flymodus-visning - forenklet kartvisning for piloter under flyging
    [Authorize(Roles = "User,Employee,Admin")]
    public class FlightmodeController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View("FlightMode");
    }
}
