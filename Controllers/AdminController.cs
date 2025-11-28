using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KartverketRegister.Controllers
{
    // Saksbehandler-dashboard for behandling av innmeldte markører
    [Authorize(Roles = "Employee,Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() => View();

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
                return Ok(new GeneralResponse(true, "Database reset successfully"));
            }
            catch (Exception e)
            {
                return Ok(new GeneralResponse(false, $"Database reset failed: {e.Message}"));
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
                    return Ok(new GeneralResponse(true, $"Marker {MarkerId} approved successfully"));
                }
                else if (Status == "Reject")
                {
                    sequel.RejectMarker(MarkerId, ReviewComment, UserId);
                    return Ok(new GeneralResponse(true, $"Marker {MarkerId} rejected successfully"));
                }
                else
                {
                    return Ok(new GeneralResponse(false, $"Marker {MarkerId} not reviewed"));
                }
            }
            catch (Exception e)
            {
                return Ok(new GeneralResponse(false, $"Marker {MarkerId} review failed: {e.Message}"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMarkers(string markerStatus)
        {
            if (Enum.TryParse(typeof(MarkerStatus), markerStatus, true, out var result))
            {
                var status = (MarkerStatus)result;

                List<Marker> MarkerList;
                AppUser appUser = await _userManager.GetUserAsync(HttpContext?.User);

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
                        Message = $"Problem fetching markers: {e.Message}"
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult DeleteMarker(int MarkerId)
        {
            SequelMarker sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                sequel.DeleteMarkerById(MarkerId);
                return Ok(new GeneralResponse(true, $"Marker {MarkerId} deleted successfully"));
            }
            catch
            {
                return Ok(new GeneralResponse(false, $"Marker {MarkerId} could not be deleted"));
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
                sequel.ApproveMarker(MarkerId, ReviewComment, UserId);
                return Ok(new GeneralResponse(true, $"Marker {MarkerId} approved successfully"));
            }
            catch
            {
                return Ok(new GeneralResponse(false, $"Marker {MarkerId} was NOT approved successfully"));
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
                return Ok(new GeneralResponse(true, $"Marker {MarkerId} rejected successfully"));
            }
            catch
            {
                return Ok(new GeneralResponse(false, $"Marker {MarkerId} was NOT rejected successfully"));
            }
        }
    }
}

