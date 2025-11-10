using Microsoft.AspNetCore.Mvc;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq.Expressions;
using KartverketRegister.Models.Other;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Authorization;
using KartverketRegister.Auth;


namespace KartverketRegister.Controllers
{
    //[Authorize(Roles = "User")] // shit works for now!!! 
    public class SuperadminController : Controller // Arver fra Controller 
    {
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
    }
}