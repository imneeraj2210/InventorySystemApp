using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using InventorySystemApp.Data;
using InventorySystemApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace InventorySystemApp.Controllers
{
    public class AccountController: Controller
    {
        private readonly InventoryRepository _repo;
        private readonly IMemoryCache _cache;

        public AccountController(InventoryRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }
        
        public IActionResult Dashboard()
        {
            const string cacheKey = "productList";

            // Try to get the products from the laptop's RAM
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Product> products))
            {
                // If not in RAM, get from Database
                products = _repo.GetAllProducts();

                // Set cache options (e.g., expire in 10 minutes)
                var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                // Save back to RAM
                _cache.Set(cacheKey, products, cacheOptions);
            }

            return View(products);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _repo.GetUser(username, password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                // 2. Sign in (Give the user the cookie)
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

                if (user.Role == "Admin") return RedirectToAction("Dashboard", "Admin"); // Send admin to Admin Dashboard
                else return RedirectToAction("Index", "Home"); // Regular users go Home

                // 3. Store in Session for extra convenience

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");

        }

        public IActionResult ManageOrder()
        {
            var orders = _repo.GetAllOrders();
            return View(orders);
        }

        [HttpPost]

        public IActionResult UpdateStatus(int orderID, string  status)
        {
            _repo.UpdateOrderStatus(orderID, status);
            return RedirectToAction("ManageOrders");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(); // This looks specifically for the .cshtml file
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Force every new registration to be a Customer
                user.Role = "Customer";

                _repo.RegisterUser(user); // Save to databse
                return RedirectToAction("Login");
            }

            return View(user);
        
        }
        
    }
}
