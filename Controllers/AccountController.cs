using InventorySystemApp.Data;
using InventorySystemApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventorySystemApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly InventoryRepository _repo;

        public AccountController(InventoryRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            // Remove Email validation for Login only
            ModelState.Remove("Email");

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                ModelState.AddModelError(
                    "Username",
                    "Username is required");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.AddModelError(
                    "Password",
                    "Password is required");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existingUser =
                _repo.GetUser(
                    user.Username!,
                    user.Password!);

            if (existingUser == null)
            {
                TempData["Error"] =
                    "Invalid username or password";

                return View(user);
            }

            var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.Name,
            existingUser.Username!),

        new Claim(
            ClaimTypes.Role,
            existingUser.Role!),

        new Claim(
            ClaimTypes.NameIdentifier,
            existingUser.Id.ToString())
    };

            var identity =
                new ClaimsIdentity(
                    claims,
                    "CookieAuth");

            await HttpContext.SignInAsync(
                "CookieAuth",
                new ClaimsPrincipal(identity));

            if (existingUser.Role == "Admin")
            {
                return RedirectToAction(
                    "Dashboard",
                    "Admin");
            }

            return RedirectToAction(
                "Index",
                "Home");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existingUser = _repo.GetUserByUsername(user.Username);

            if (existingUser != null)
            {
                TempData["Error"] =
                    "Username already exists.";

                return View(user);
            }

            if (ModelState.IsValid)
            {
                user.Role = "Customer";

                _repo.RegisterUser(user);

                TempData["Success"] = "Registration successful. Please login.";

                return RedirectToAction("Login");
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");

            return RedirectToAction("Login");
        }
    }
}
