using KartverketRegister.Auth;
using KartverketRegister.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KartverketRegister.Controllers
{
    // Autentisering: innlogging, registrering, utlogging
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
        public IActionResult AccessDenied() => View();

        [HttpGet]
        public IActionResult Login() => View();

        // isPersistent: false = session cookie (slettes ved lukking av browser)
        // lockoutOnFailure: false = ingen lockout ved feil passord
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LoginHandle(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Json(new GeneralResponse(false, "User not found"));

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

            return result.Succeeded
                ? Json(new GeneralResponse(true, "Logged in successfully"))
                : Json(new GeneralResponse(false, "Invalid password"));
        }

        [HttpGet]
        public IActionResult Register() => View();

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RegisterHandle(string username, string lastname, string org, string password, string email)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
                return Json(new GeneralResponse(false, "Email already in use"));

            var user = new AppUser
            {
                Name = email,
                FirstName = username,
                LastName = lastname,
                Organization = org,
                UserName = email,
                UserType = "User",  // Default rolle
                Password = password,
                Email = email
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Json(new GeneralResponse(true, "Registered successfully"));
            }

            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Json(new GeneralResponse(false, $"Registration failed: {errors}"));
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
