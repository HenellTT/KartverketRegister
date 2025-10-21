using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq.Expressions;
using KartverketRegister.Models.Other;
using System.Diagnostics.Eventing.Reader;


namespace KartverketRegister.Controllers
{

    public class AdminController : Controller // Arver fra Controller for å håndtere markører
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();

        }
        [HttpGet]
        public IActionResult ResetDB()
        {
            try
            {
                SequelInit sequel = new SequelInit(Constants.DataBaseIp, Constants.DataBaseName);
                Constants.ResetDbOnStartup = true;
                sequel.conn.Open();
                sequel.InitDb();
                sequel.conn.Close();
                Constants.ResetDbOnStartup = false;
                return Ok(new GeneralResponse(true, "Database Resetted Successfully"));
            } catch (Exception e)
            {
                return Ok(new GeneralResponse(true, $"Database Reset failed: {e.Message}"));
            }

        }

        [HttpPost]
        public IActionResult Review(int markerId)
        {
            SequelMarker sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            Marker Mrk = sequel.FetchMarkerById(markerId);
            
            if (Mrk == null)
            {
                return NotFound();
            }
            Console.WriteLine($"Mrk!!: {markerId}");
            return View(Mrk);
        }
        [HttpPost]
        public IActionResult HandleReview(int MarkerId, string ReviewComment, string Status)
        {
            SequelMarker sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            Console.WriteLine($"Mid: {MarkerId}, RC: {ReviewComment} St: {Status}");
            try
            {
                if (Status == "Approve")
                {
                    sequel.ApproveMarker(MarkerId, ReviewComment);
                    return Ok(new GeneralResponse(true, $"Marker {MarkerId} Approved successfully"));

                }
                else if (Status == "Reject")
                {
                    sequel.RejectMarker(MarkerId, ReviewComment);
                    return Ok(new GeneralResponse(true, $"Marker {MarkerId} Rejected successfully"));
                }
                else
                {
                    return Ok(new GeneralResponse(false, $"Marker {MarkerId} not reviewed"));
                }
            }
            catch (Exception e)
            {
                return Ok(new GeneralResponse(false, $"Marker {MarkerId} was NOT reviewed successfully! Error: {e.Message}"));
            }
        }

        [HttpGet]
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

                }
                catch (Exception e)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"Problem fetching markers! E: {e}"
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
        [HttpGet]
        public IActionResult DeleteMarker(int MarkerId)
        {
            SequelMarker sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                sequel.DeleteMarkerById(MarkerId);
                return Ok(new GeneralResponse(true,$"Marker ${MarkerId} removed successfully"));
            }
            catch
            {
                return Ok(new GeneralResponse(false, $"Marker ${MarkerId} was NOT removed successfully"));
            }
        }
        
        [HttpPost]
        public IActionResult Approve(int MarkerId, string ReviewComment)
        {
            SequelMarker sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                sequel.ApproveMarker(MarkerId,ReviewComment);
                return Ok(new GeneralResponse(true, $"Marker ${MarkerId} approved successfully"));
            }
            catch
            {
                return Ok(new GeneralResponse(false, $"Marker ${MarkerId} was NOT approved successfully"));
            }
        }
        [HttpPost]
        public IActionResult Reject(int MarkerId, string ReviewComment)
        {
            SequelMarker sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                sequel.RejectMarker(MarkerId, ReviewComment);
                return Ok(new GeneralResponse(true, $"Marker ${MarkerId} approved successfully"));
            }
            catch
            {
                return Ok(new GeneralResponse(false, $"Marker ${MarkerId} was NOT approved successfully"));
            }
        }
    }
}