using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Models.Other;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace KartverketRegister.Controllers
{
    // Generelle API-endepunkter: høydedata, varsler
    [Authorize(Roles = "Employee,Admin,User")]
    public class ApiController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ApiController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() => Ok(new GeneralResponse(true, "API is running"));

        // Henter høyde over havet fra Kartverkets HøydeData API
        // Konverterer WGS84 (lat/lng) til UTM33N før API-kall
        [HttpGet]
        public async Task<IActionResult> GetHeight(double lat, double lng)
        {
            var wgs84 = GeographicCoordinateSystem.WGS84;
            var utm33N = ProjectedCoordinateSystem.WGS84_UTM(33, true);

            var transformFactory = new CoordinateTransformationFactory();
            var transform = transformFactory.CreateFromCoordinateSystems(wgs84, utm33N);

            double[] utmCoords = transform.MathTransform.Transform(new double[] { lng, lat });
            double e = utmCoords[0];  // Easting (x)
            double n = utmCoords[1];  // Northing (y)

            string url = LinkGeneratorBrr.HoydeDataCoords(e, n);
            double? height = await HeightFetcher.GetHeightFromUrlAsync(url);

            return Ok(new { url, e, n, lat, lng, height });
        }

        // Henter alle varsler for innlogget bruker
        [HttpGet]
        public IActionResult GetNotifications()
        {
            string userIdString = _userManager.GetUserId(HttpContext?.User);
            int userId = int.TryParse(userIdString, out var id) ? id : 0;

            try
            {
                List<NotificationModel> notifications = Notificator.GetNotificationsByUserId(userId);
                return Json(new GeneralResponse(true, "Notifications fetched", notifications));
            }
            catch
            {
                return Json(new GeneralResponse(false, "No notifications found"));
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult MarkNotificationAsRead([FromBody] NotificationRequest request)
        {
            string userIdString = _userManager.GetUserId(HttpContext?.User);
            int userId = int.TryParse(userIdString, out var id) ? id : 0;

            try
            {
                Notificator.SetToRead(request.NotificationId, userId);
                return Json(new GeneralResponse(true, "Notification marked as read"));
            }
            catch
            {
                return Json(new GeneralResponse(false, "Could not mark notification as read"));
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult DeleteNotification([FromBody] NotificationRequest request)
        {
            string userIdString = _userManager.GetUserId(HttpContext?.User);
            int userId = int.TryParse(userIdString, out var id) ? id : 0;

            try
            {
                Notificator.DeleteNotificationByUser(userId, request.NotificationId);
                return Json(new GeneralResponse(true, "Notification deleted"));
            }
            catch
            {
                return Json(new GeneralResponse(false, "Could not delete notification"));
            }
        }
    }
}
