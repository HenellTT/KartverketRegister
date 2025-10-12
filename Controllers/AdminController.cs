using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq.Expressions;
using KartverketRegister.Models.Other;


namespace KartverketRegister.Controllers
{

    public class AdminController : Controller // Arver fra Controller for å håndtere markører
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAllMarkers(string markerStatus)
        {
            if (Enum.TryParse(typeof(MarkerStatus), markerStatus, true, out var result))
            {
                // Enum.TryParse succeeded, 'result' is of type object
                var status = (MarkerStatus)result;

                List<Marker> MarkerList;

                try
                {
                    SequelAdmin sequel = new SequelAdmin(Constants.DataBaseIp, Constants.DataBaseName);
                    MarkerList = sequel.FetchAllMarkers(markerStatus);

                } catch
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Problem fetching markers!"
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Name = status.ToString(),
                    Value = (int)status,
                    Markers = MarkerList
                });
            }
            else
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = $"'{markerStatus}' is not a valid MarkerStatus value."
                });
            }
        }

    }
}