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

        // Nullstiller databasen - kun for utvikling/testing
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult ResetDB()
        {
            try
            {
                var sequel = new SequelInit(Constants.DataBaseIp, Constants.DataBaseName);
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

        // Viser review-side for en spesifikk markør
        [HttpPost]
        public IActionResult Review(int markerId)
        {
            var sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            Marker marker = sequel.FetchMarkerById(markerId);

            if (marker == null)
                return NotFound();

            return View(marker);
        }

        // Behandler godkjenning/avslag fra review-siden
        [HttpPost]
        public async Task<IActionResult> HandleReview(int markerId, string reviewComment, string status)
        {
            var sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            AppUser appUser = await _userManager.GetUserAsync(HttpContext?.User);
            int userId = appUser.Id;

            try
            {
                return status switch
                {
                    "Approve" => ApproveMarkerInternal(sequel, markerId, reviewComment, userId),
                    "Reject" => RejectMarkerInternal(sequel, markerId, reviewComment, userId),
                    _ => Ok(new GeneralResponse(false, $"Marker {markerId} not reviewed - invalid status"))
                };
            }
            catch (Exception e)
            {
                return Ok(new GeneralResponse(false, $"Marker {markerId} review failed: {e.Message}"));
            }
        }

        // Henter markører filtrert på status - kun de tildelt innlogget saksbehandler
        [HttpGet]
        public async Task<IActionResult> GetAllMarkers(string markerStatus)
        {
            if (!Enum.TryParse(typeof(MarkerStatus), markerStatus, true, out var result))
            {
                return BadRequest(new { Success = false, Message = $"'{markerStatus}' is not a valid MarkerStatus" });
            }

            var status = (MarkerStatus)result;
            AppUser appUser = await _userManager.GetUserAsync(HttpContext?.User);

            try
            {
                var sequel = new SequelAdmin(Constants.DataBaseIp, Constants.DataBaseName);
                List<Marker> markers = sequel.FetchAllMarkers(markerStatus, appUser.Id);

                return Ok(new
                {
                    Success = true,
                    Name = status.ToString(),
                    Value = (int)status,
                    Markers = markers
                });
            }
            catch (Exception e)
            {
                return BadRequest(new { Success = false, Message = $"Failed to fetch markers: {e.Message}" });
            }
        }

        // Sletter en markør permanent
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult DeleteMarker(int markerId)
        {
            var sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            try
            {
                sequel.DeleteMarkerById(markerId);
                return Ok(new GeneralResponse(true, $"Marker {markerId} deleted successfully"));
            }
            catch
            {
                return Ok(new GeneralResponse(false, $"Marker {markerId} could not be deleted"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int markerId, string reviewComment)
        {
            AppUser appUser = await _userManager.GetUserAsync(HttpContext?.User);
            var sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            return ApproveMarkerInternal(sequel, markerId, reviewComment, appUser.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int markerId, string reviewComment)
        {
            AppUser appUser = await _userManager.GetUserAsync(HttpContext?.User);
            var sequel = new SequelMarker(Constants.DataBaseIp, Constants.DataBaseName);
            return RejectMarkerInternal(sequel, markerId, reviewComment, appUser.Id);
        }

        // Hjelpemetoder for godkjenning/avslag
        private IActionResult ApproveMarkerInternal(SequelMarker sequel, int markerId, string reviewComment, int userId)
        {
            try
            {
                sequel.ApproveMarker(markerId, reviewComment, userId);
                return Ok(new GeneralResponse(true, $"Marker {markerId} approved successfully"));
            }
            catch (Exception e)
            {
                return Ok(new GeneralResponse(false, $"Marker {markerId} could not be approved: {e.Message}"));
            }
        }

        private IActionResult RejectMarkerInternal(SequelMarker sequel, int markerId, string reviewComment, int userId)
        {
            try
            {
                sequel.RejectMarker(markerId, reviewComment, userId);
                return Ok(new GeneralResponse(true, $"Marker {markerId} rejected successfully"));
            }
            catch (Exception e)
            {
                return Ok(new GeneralResponse(false, $"Marker {markerId} could not be rejected: {e.Message}"));
            }
        }
    }
}

