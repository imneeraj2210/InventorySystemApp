using InventorySystemApp.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventorySystemApp.Controllers
{
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
            var claim =
                User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier);

            if (claim == null)
                return Unauthorized();

            int userId =
                int.Parse(claim.Value);

            bool success =
                _repo.PlaceOrder(
                    userId,
                    productId,
                    quantity);

            if (success)
                TempData["Success"] =
                    "Order placed successfully!";
            else
                TempData["Error"] =
                    "Order failed or insufficient stock!";

            return RedirectToAction(
                "Index",
                "Home");
        }

        public IActionResult MyOrders()
        {
            var claim =
                User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier);

            if (claim == null)
                return Unauthorized();

            int userId =
                int.Parse(claim.Value);

            var orders =
                _repo.GetUserOrders(userId);

            return View(orders);
        }

        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            _repo.CancelOrder(id);

            TempData["Success"] =
                "Order cancelled successfully!";

            return RedirectToAction("MyOrders");
        }

    }
}
