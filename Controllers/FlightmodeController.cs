using KartverketRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;


namespace KartverketRegister.Controllers
{
    [Authorize(Roles = "User,Employee,Admin")]

    public class FlightmodeController : Controller
    {
        public IActionResult Index()
        {
            return View("FlightMode"); //returnerer viewet FlightMode.cshtml (FlightMode siden)
		}
    
    }
}