using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Models.Other;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace KartverketRegister.Controllers
{
    [Authorize(Roles = "Employee,Admin")] 
    public class AdminController : Controller 
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
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
            return View(Mrk);
        }
        [HttpPost]
        public async Task<IActionResult> HandleReview(int MarkerId, string ReviewComment, string Status)
        {
            SequelMarker sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            AppUser appUser = await _userManager.GetUserAsync(HttpContext?.User);
            int UserId = appUser.Id;

            try
            {
                if (Status == "Approve")
                {
                    sequel.ApproveMarker(MarkerId, ReviewComment, UserId);
                    return Ok(new GeneralResponse(true, $"Marker {MarkerId} Approved successfully"));

                }
                else if (Status == "Reject")
                {
                    sequel.RejectMarker(MarkerId, ReviewComment, UserId);
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
        public async Task<IActionResult> GetAllMarkers(string markerStatus) // RESTRICT TO ONLY ASSIGNED MARKERS 
        {
            if (Enum.TryParse(typeof(MarkerStatus), markerStatus, true, out var result))
            {
                // Enum.TryParse succeeded, 'result' is of type object
                var status = (MarkerStatus)result;

                List<Marker> MarkerList;
                AppUser appUser = await _userManager.GetUserAsync(HttpContext?.User);
                Console.WriteLine($"GetAllMarkers requested by UserId {appUser.Id}");

                try
                {
                    SequelAdmin sequel = new SequelAdmin(Constants.DataBaseIp, Constants.DataBaseName);
                    MarkerList = sequel.FetchAllMarkers(markerStatus, appUser.Id);

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
        public async Task<IActionResult> Approve(int MarkerId, string ReviewComment)
        {
            AppUser appUser = await _userManager.GetUserAsync(HttpContext?.User);
            int UserId = appUser.Id;

            SequelMarker sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                sequel.ApproveMarker(MarkerId,ReviewComment, UserId);
                return Ok(new GeneralResponse(true, $"Marker ${MarkerId} approved successfully"));
            }
            catch
            {
                return Ok(new GeneralResponse(false, $"Marker ${MarkerId} was NOT approved successfully"));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Reject(int MarkerId, string ReviewComment)
        {
            AppUser appUser = await _userManager.GetUserAsync(HttpContext?.User);
            int UserId = appUser.Id;

            SequelMarker sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                sequel.RejectMarker(MarkerId, ReviewComment, UserId);
                return Ok(new GeneralResponse(true, $"Marker ${MarkerId} approved successfully"));
            }
            catch
            {
                return Ok(new GeneralResponse(false, $"Marker ${MarkerId} was NOT approved successfully"));
            }
        }
    }
}