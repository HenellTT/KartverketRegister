using KartverketRegister.Auth;
using KartverketRegister.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
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

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login(string role)
        {
            ViewBag.Role = role;
            return View();
        }

        // POST: /Auth/LoginHandle
        [HttpPost]
        public async Task<IActionResult> LoginHandle(string email, string password)
        {
            AppUser user;
            try
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Json(new GeneralResponse(false, "User not found"));
            } catch (Exception e)
            {
                return Json(new GeneralResponse(false, $"Email not found"));

            }



            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
                return Json(new GeneralResponse(true, "Logged in successfully"));

            result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var currentRole = roles.FirstOrDefault();

                // Redirect basert på rolle
                string redirectUrl = currentRole switch
                {
                    "Admin" => "/Admin/Dashboard",
                    "Pilot" => "/Pilot/Overview",
                    _ => "/Home/Index"
                };

                return Json(new GeneralResponse(true, redirectUrl));
            }

            return Json(new GeneralResponse(false, "Invalid password"));
        }

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/RegisterHandle
        [HttpPost]
        public async Task<IActionResult> RegisterHandle(string username, string lastname, string org, string password, string email, string role)
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
                    UserType = role, // default role
                    Password = password,
                    Email = email
                };
            } catch (Exception e)
            {
                return Json(new GeneralResponse(false, $"You must fill out all the fields! {e}"));
            }
            

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
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
            return RedirectToAction("Index","Home");
        }
    }
}
