using KartverketRegister.Auth;
using KartverketRegister.Models;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace KartverketRegister.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }



        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/LoginHandle
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LoginHandle(string email, string password)
        {
            AppUser user;
            try
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Json(new GeneralResponse(false, "User not found"));
            }
            catch (Exception e)
            {
                return Json(new GeneralResponse(false, $"Email not found"));

            }



            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
                return Json(new GeneralResponse(true, "Logged in successfully"));

            return Json(new GeneralResponse(false, "Invalid password"));
        }

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/RegisterHandle
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RegisterHandle(string username, string lastname, string org, string password, string email)
        {
            AppUser user;
            try
            {
                var existing = await _userManager.FindByEmailAsync(email);

                if (existing != null)
                    return Json(new GeneralResponse(false, "Email already in use!"));
                user = new AppUser
                {
                    Name = email,
                    FirstName = username,
                    LastName = lastname,
                    Organization = org,
                    UserName = email,
                    UserType = "User", // default role
                    Password = password,
                    Email = email
                };
            }
            catch (Exception e)
            {
                return Json(new GeneralResponse(false, $"You must fill out all the fields!"));
            }


            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Json(new GeneralResponse(true, "Registered successfully"));
            }

            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Json(new GeneralResponse(false, $"Registration failed: {errors}"));
        }


        // GET: /Auth/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        
            

            
    }
}
