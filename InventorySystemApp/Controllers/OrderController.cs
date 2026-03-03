using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using InventorySystemApp.Data;

namespace InventorySystemApp.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly InventoryRepository _repo;

        public OrderController(InventoryRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public IActionResult PlaceOrder(int productId, int quantity)
        {
            // 1. Fetch the actual stock from the database
            var product = _repo.GetProductById(productId);

            if (product == null) return NotFound();

            // 2. The Backend Guard: Check if enough stock exists
            if (quantity > product.Stock)
            {
                TempData["Error"] = $"Sorry, only {product.Stock} item left in the stock!";
                return RedirectToAction("Index", "Home");
            }

            // 3. If valid, proceed with the order
            // This gets the logged-in User ID from claims
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Unauthorized();

            int userId = int.Parse(claim.Value);

            // FIX: Pass userId first, then productId
            bool success = _repo.PlaceOrder(userId, productId, quantity);

            if (success) TempData["Success"] = "Order placed successfully!";
            else TempData["Error"] = "Order failed. Check stock levels.";

            
          
            return RedirectToAction("Index", "Home");
        }
        
        /*public IActionResult MyOrders()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (claim == null) return Unauthorized();

            int userId = int.Parse(claim.Value);
            var orders = _repo.GetUserOrders(userId); // Call repository method.
            return View(orders);
        }*/

        [HttpGet]
        public IActionResult MyOrders() // Change IActionResult to string
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (claim == null) return Unauthorized();

            int userId = int.Parse(claim.Value);
            var orders = _repo.GetUserOrders(userId); // Call repository method.
            return View(orders);
        }
    }
}