using InventorySystemApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventorySystemApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly InventoryRepository _repo;

        public CartController(
            InventoryRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var claim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (claim == null)
                return Unauthorized();

            int userId =
                int.Parse(claim.Value);

            _repo.AddToCart(
                userId,
                productId,
                quantity);

            TempData["Success"] =
                "Item added to cart!";

            return RedirectToAction(
                "Index",
                "Home");
        }

        public IActionResult Index()
        {
            var claim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (claim == null)
                return Unauthorized();

            int userId =
                int.Parse(claim.Value);

            var cartItems =
                _repo.GetCartItems(userId);

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult Remove(
            int id)
        {
            _repo.RemoveCartItem(id);

            TempData["Success"] =
                "Item removed from cart.";

            return RedirectToAction(
                "Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            if (quantity < 1)
            {
                quantity = 1;
            }

            _repo.UpdateCartQuantity(id, quantity);

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            int userId =
                int.Parse(
                    User.FindFirst(
                        ClaimTypes.NameIdentifier)!
                        .Value);

            _repo.CheckoutCart(userId);

            TempData["Success"] =
                "Order placed successfully!";

            return RedirectToAction(
                "MyOrders",
                "Order");
        }
    }
}