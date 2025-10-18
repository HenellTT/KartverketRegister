using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using KartverketRegister.Data;
using System.Collections.Generic;

namespace KartverketRegister.Controllers
{
    public class RegistryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Registry()
        {
            var Markers = _context.Markers.ToList();
            var TempMarkers = _context.TempMarkers.ToList();

            var viewModel = new RegistryViewModel
            {
                Markers = Markers,
                TempMarkers = TempMarkers
            };

            // Explicit path to the existing view under Views/Home
            return View("~/Views/Home/Registry.cshtml", viewModel);
        }
    }
}