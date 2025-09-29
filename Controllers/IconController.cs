using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Utils;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;


namespace KartverketRegister.Controllers
{

    public class IconController : Controller
    {
        //private int MsgLimit = 15;
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult GetIcons()
        {
            var iconDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/icons");
            var icons = Directory.GetFiles(iconDir)
                         .Select(f => Path.GetFileNameWithoutExtension(f))
                         .ToList();


            return Json(icons);
        }
        
    }
}