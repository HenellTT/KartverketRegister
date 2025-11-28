using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Models.Other;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KartverketRegister.Controllers
{
    // Superadmin-dashboard - brukeradministrasjon og tildeling av innmeldinger
    [Authorize(Roles = "Admin")]
    public class SuperadminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public SuperadminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult ManageUsers() => View();

        [HttpGet]
        public IActionResult AssignSubmissions() => View();

        [HttpGet("Superadmin/ManageUsers/{userId}")]
        public IActionResult ManageUsersDetails(int userId)
        {
            var seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
            AppUserDto user = seq.FetchUser(userId);
            return View("ManageUserDetails", user);
        }

        // Endrer brukerrolle (User/Employee/Admin)
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SetUserRole([FromBody] AppUserDto userData)
        {
            var seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
            GeneralResponse response = seq.SetUserRole(userData);
            return Ok(response);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult DeleteUser([FromBody] AppUserDto userData)
        {
            var seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
            GeneralResponse response = seq.DeleteUser(userData.Id);
            return Ok(response);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SendNotification([FromBody] AppUserDto userData)
        {
            var seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
            GeneralResponse response = seq.SendNotification(userData.Id, userData.Email);
            return Ok(response);
        }

        // Henter brukerliste med valgfri navnefiltrering
        [HttpGet]
        public IActionResult FetchUsers(string fullName = "")
        {
            try
            {
                var seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
                List<AppUserDto> users = seq.UserFetcher(fullName);
                return Ok(new GeneralResponse(true, "Users fetched successfully", users));
            }
            catch (Exception ex)
            {
                return Ok(new GeneralResponse(false, $"Failed to fetch users: {ex.Message}"));
            }
        }

        // Henter kun ansatte (Employee-rolle)
        [HttpGet]
        public IActionResult FetchEmployees(string fullName = "")
        {
            try
            {
                var seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
                List<AppUserDto> users = seq.AdvUserFetcher("Employee", fullName);
                return Ok(new GeneralResponse(true, "Employees fetched successfully", users));
            }
            catch (Exception ex)
            {
                return Ok(new GeneralResponse(false, $"Failed to fetch employees: {ex.Message}"));
            }
        }

        // Henter markører som ikke er tildelt en saksbehandler
        [HttpGet]
        public IActionResult FetchUnassignedMarkers()
        {
            try
            {
                var seq = new SequelSuperAdmin();
                List<Marker> markers = seq.FetchAllUnassignedMarkers();
                return Json(new GeneralResponse(true, "Unassigned markers fetched", markers));
            }
            catch (Exception ex)
            {
                return Json(new GeneralResponse(false, $"Failed to fetch markers: {ex.Message}"));
            }
        }

        // Henter alle markører filtrert på status
        [HttpGet]
        public IActionResult FetchAllMarkers(string markerStatus)
        {
            try
            {
                var seq = new SequelSuperAdmin();
                List<Marker> markers = seq.FetchAllMarkers(markerStatus);
                return Ok(new GeneralResponse(true, $"Markers with status '{markerStatus}' fetched", markers));
            }
            catch (Exception ex)
            {
                return Json(new GeneralResponse(false, $"Failed to fetch markers: {ex.Message}"));
            }
        }

        // Fjerner alle tildelinger
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult UnAssignAll()
        {
            var seq = new SequelSuperAdmin();
            return Json(seq.UnAssignAll());
        }

        // Tildeler innmeldinger til en saksbehandler
        // Validerer at mottaker faktisk er Employee før tildeling
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AssignReviews([FromBody] List<ReviewAssign> assignedReviews)
        {
            var firstUserId = assignedReviews?.FirstOrDefault()?.UserId;

            if (firstUserId == null)
                return Json(new GeneralResponse(false, "No reviews to assign"));

            // Verifiser at mottaker er en ansatt
            AppUser? selectedEmployee = await _userManager.FindByIdAsync(firstUserId.ToString());

            if (selectedEmployee?.UserType != "Employee")
                return Json(new GeneralResponse(false, "Target user is not an Employee"));

            var seq = new SequelSuperAdmin();
            seq.Open();

            try
            {
                int succeeded = 0;
                int failed = 0;

                foreach (ReviewAssign ra in assignedReviews)
                {
                    GeneralResponse r = seq.AssignReview(ra);
                    if (r.Success)
                        succeeded++;
                    else
                        failed++;
                }

                // Send varsel til saksbehandler
                Notificator.SendNotification(
                    Convert.ToInt32(firstUserId),
                    $"You have been assigned to review {succeeded} submissions<br><a href='/Admin'><button>To Submissions</button></a>",
                    "Info"
                );

                return Json(new GeneralResponse(true, "Reviews assigned successfully", new { Success = succeeded, Fail = failed }));
            }
            catch (Exception ex)
            {
                return Json(new GeneralResponse(false, $"Assignment failed: {ex.Message}"));
            }
            finally
            {
                seq.Close();  // Sikrer at connection lukkes uansett utfall
            }
        }
    }
}
