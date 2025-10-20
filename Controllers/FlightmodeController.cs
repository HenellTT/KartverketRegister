using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Utils;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;


namespace KartverketRegister.Controllers
{

    public class FlightmodeController : Controller
    {
        public IActionResult Index()
        {
            return View("FlightMode"); //returnerer viewet FlightMode.cshtml (FlightMode siden)
		}
    
    }
}