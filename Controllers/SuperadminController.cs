using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Models.Other;
using KartverketRegister.Utils;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq.Expressions;

namespace KartverketRegister.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SuperadminController : Controller // Arver fra Controller 
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public SuperadminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();

        }
        [HttpGet]
        public IActionResult ManageUsers()
        {
            return View();

        }
        [HttpGet]
        public IActionResult AssignSubmissions()
        {
            return View();

        }
        

        [HttpGet("Superadmin/ManageUsers/{UserId}")]
        public IActionResult ManageUsersDetails(int UserId)
        {
            {
                SequelSuperAdmin seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
                AppUserDto user = seq.FetchUser(UserId);
                return View("ManageUserDetails", user);


            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SetUserRole([FromBody] AppUserDto UserData)
        {
            SequelSuperAdmin seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
            GeneralResponse response = seq.SetUserRole(UserData);
            return Ok(response);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult DeleteUser([FromBody] AppUserDto UserData)
        {
            SequelSuperAdmin seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
            GeneralResponse response = seq.DeleteUser(UserData.Id);
            return Ok(response);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SendNotification([FromBody] AppUserDto UserData)
        {
            SequelSuperAdmin seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
            GeneralResponse response = seq.SendNotification(UserData.Id, UserData.Email);
            return Ok(response);
        }
        [HttpGet]
        public IActionResult FetchUsers(string FullName = "") {
            try
            {
                SequelSuperAdmin seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
                List<AppUserDto> Users = seq.UserFetcher(FullName);
                return Ok(new GeneralResponse(true, "User list was indeed found", Users));
            } catch (Exception ex)
            {
                return Ok(new GeneralResponse(false,$"No users found {ex.Message} "));
            }
        }
        [HttpGet]
        public IActionResult FetchEmployees(string FullName = "")
        {
            try
            {
                SequelSuperAdmin seq = new SequelSuperAdmin(Constants.DataBaseIp, Constants.DataBaseName);
                List<AppUserDto> Users = seq.AdvUserFetcher("Employee",FullName);
                return Ok(new GeneralResponse(true, "User list was indeed found", Users));
            }
            catch (Exception ex)
            {
                return Ok(new GeneralResponse(false, $"No users found {ex.Message} "));
            }
        }
        [HttpGet]
        public IActionResult FetchUnassignedMarkers()
        {
            try
            {
                SequelSuperAdmin seq = new SequelSuperAdmin();
                List<Marker> mrks = seq.FetchAllUnassignedMarkers();
                return Json(new GeneralResponse(true, "Here are yo markers bro", mrks));
            }
            catch (Exception ex)
            {
                return Json(new GeneralResponse(false, $"error: {ex.Message}"));
            }
        }
        [HttpGet]
        public IActionResult FetchAllMarkers(string markerStatus)
        {
            try
            {
                SequelSuperAdmin seq = new SequelSuperAdmin();
                List<Marker> mrks = seq.FetchAllMarkers(markerStatus);
                
                return Ok(new
                {
                    Success = true,
                    Name = markerStatus.ToString(),
                    Value = 69,
                    Markers = mrks
                });
            }
            catch (Exception ex)
            {
                return Json(new GeneralResponse(false, $"error: {ex.Message}"));
            }
        }
        [HttpGet]
        public IActionResult UnAssignAll()
        {
            SequelSuperAdmin seq = new SequelSuperAdmin();
            return Json(seq.UnAssignAll());
        }
        [HttpPost]
        public async Task<IActionResult> PostAssignReviews([FromBody] List<ReviewAssign> AssignedReviews) {

            var firstUserId = AssignedReviews?.FirstOrDefault()?.UserId;

            if (firstUserId == null)
                return Json(new GeneralResponse(false, "AssignedReviews list is empty"));

            // lar ikke Assigne reviews til de som ikke er employee
            AppUser SelectedEmployee = await _userManager.FindByIdAsync(firstUserId.ToString());
        
            if (SelectedEmployee?.UserType != "Employee")
                return Json(new GeneralResponse(false, "Fr if you get this reply, you must've really played with the feelings of our security measures. btw, User is not an Employee"));
        
            SequelSuperAdmin seq = new SequelSuperAdmin();
            seq.Open();
            try
            {
                
                int Succeeded = 0;
                int Failed = 0;
                foreach (ReviewAssign RA in AssignedReviews)
                {
                    GeneralResponse r = seq.AssignReview(RA);
                    Console.WriteLine($"[RA] uid:{RA.UserId} mid:{RA.MarkerId}");
                    if (r.Success) Succeeded++;
                    else { 
                        Failed++;
                        Console.WriteLine($"[RA] seq error: {r.Message}");
                    }
                }
                Notificator.SendNotification(Convert.ToInt32(firstUserId),$"You have been assigned to review {Succeeded} submissions<br><a href='/Admin'><button>To Submissions</button></a>", "Info");
                return Json(new GeneralResponse(true, $"Everything gucci", new { Success = Succeeded, Fail = Failed }));

            }
            catch (Exception ex)
            {
                return Json(new GeneralResponse(false, $"Something went wrong: {ex.Message}"));
            }
            seq.Close();

        }
    }
}