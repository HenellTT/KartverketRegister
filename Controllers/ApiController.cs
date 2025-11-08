using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Threading.Tasks;

namespace KartverketRegister.Controllers
{
    public class ApiController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public ApiController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new GeneralResponse(true,"Api is apiing"));

        }

        [HttpGet]
        // Stjeler MOH value fra HoydeData sin api som en ekte sigma 😎
        public async Task<IActionResult> GetHeight(double lat, double lng)
        {
            var wgs84 = GeographicCoordinateSystem.WGS84;
            var utm33N = ProjectedCoordinateSystem.WGS84_UTM(33, true);

            var transformFactory = new CoordinateTransformationFactory();
            var transform = transformFactory.CreateFromCoordinateSystems(wgs84, utm33N);

            double[] utmCoords = transform.MathTransform.Transform(new double[] { lng, lat });
            double e = utmCoords[0];  // x
            double n = utmCoords[1]; // y
            

            string url = LinkGeneratorBrr.HoydeDataCoords(e, n);

            double? height = await HeightFetcher.GetHeightFromUrlAsync(url);

            return Ok( new { 
                url = url,
                e = e, 
                n = n, 
                lat = lat, 
                lng = lng, 
                height = height
            });
        }
        [HttpGet]
        public IActionResult GetNotifications()
        {
            string UserIdString = _userManager.GetUserId(HttpContext?.User);
            int UserId = int.TryParse(UserIdString, out var id) ? id : 0;
            try
            {
                List<NotificationModel> Notifications = Notificator.GetNotificationsByUserId(UserId);
                return Json(new GeneralResponse(true, $"Here are your msgs bro", Notifications));
            } catch
            {
                return Json(new GeneralResponse(false, "No notifications fo u bro"));
            }
            

            
        }
        [HttpGet]
        public IActionResult SendNotification(int userid, string msg) // ONLY FOR TESTING RESTRICT or DELETE l8r  
        {
            try
            {
                Notificator.SendNotification(userid, msg, "Info");
                return Json(new GeneralResponse(true, $"Message {msg} sent to {userid}"));
            } catch
            {
                return Json(new GeneralResponse(false, $"Failed sending msg: {msg} to {userid}"));

            }
        }

    }
}
