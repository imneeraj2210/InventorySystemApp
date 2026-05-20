using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventorySystemApp.Data;
using InventorySystemApp.Models;
using Npgsql;


namespace InventorySystemApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly InventoryRepository _repo;

        public AdminController(InventoryRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Dashboard(
     int page = 1)
        {
            int pageSize = 5;

            ViewBag.TotalProducts =
                _repo.GetTotalProducts();

            ViewBag.TotalUsers =
                _repo.GetTotalUsers();

            ViewBag.TotalOrders =
                _repo.GetTotalOrders();

            ViewBag.PendingOrders =
                _repo.GetPendingOrders();

            var products =
                _repo.GetProductsPaged(
                    page,
                    pageSize);

            int totalProducts =
                _repo.GetProductCount();

            ViewBag.CurrentPage =
                page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    (double)totalProducts /
                    pageSize);

            return View(products);
        }
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = _repo.GetProductById(id);

            if (product == null) { return NotFound(); }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var existingProduct =
                _repo.GetProductById(product.Id);

            if (imageFile != null &&
                imageFile.Length > 0)
            {
                string fileName =
                    Guid.NewGuid() +
                    Path.GetExtension(
                        imageFile.FileName);

                string path =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images",
                        fileName);

                using var stream =
                    new FileStream(
                        path,
                        FileMode.Create);

                await imageFile.CopyToAsync(stream);

                product.ImageUrl =
                    "/images/" + fileName;
            }
            else
            {
                // Keep old image
                product.ImageUrl =
                    existingProduct?.ImageUrl;
            }

            _repo.UpdateProduct(product);

            TempData["Success"] =
                "Product updated successfully!";

            return RedirectToAction(
                "Dashboard");
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                _repo.DeleteProduct(id);

                TempData["Success"] =
                    "Product deleted successfully.";
            }
            catch
            {
                TempData["Error"] =
                    "Cannot delete product because orders exist.";
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                //Force default role
                user.Role = "Customer";

                _repo.RegisterUser(user);

                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Orders()
        {
            var order = _repo.GetAllOrders();
            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int orderID, string status)
        {
            _repo.UpdateOrderStatus(
                orderID,
                status);

            TempData["Success"] =
                "Order updated successfully!";

            return RedirectToAction("Orders");
        }

        
        [HttpPost]
        public async Task<IActionResult> AddProduct( Product product, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null &&
                    imageFile.Length > 0)
                {
                    string fileName =
                        Guid.NewGuid().ToString() +
                        Path.GetExtension(
                            imageFile.FileName);

                    string path =
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/images",
                            fileName);

                    using (var stream =
                        new FileStream(path,
                            FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    product.ImageUrl =
                        "/images/" + fileName;
                }

                _repo.AddProduct(product);

                TempData["Success"] =
                    "Product added successfully!";

                return RedirectToAction("Dashboard");
            }

            return View(product);
        }

        [HttpGet]
        public IActionResult WebsiteSettings()
        {
            var setting = _repo.GetWebsiteSettings();

            return View(setting);
        }

        [HttpPost]
        public IActionResult WebsiteSettings(WebsiteSetting setting, IFormFile? logoFile)
        {
            if(logoFile != null)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                string fileName = Guid.NewGuid() + Path.GetExtension(logoFile.FileName);

                string filePath = Path.Combine(folder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);

                logoFile.CopyTo(stream);

                setting.LogoUrl = "/images/" + fileName;
            }

            else
            {
                var oldSetting = _repo.GetWebsiteSettings();

                setting.LogoUrl = oldSetting.LogoUrl;
            }

            _repo.UpdateWebsiteSettings(setting);

            TempData["Success"] = "Website Setting updated!!";

            return RedirectToAction("WebsiteSettings");
        }
    }
}
