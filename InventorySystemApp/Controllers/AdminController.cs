using InventorySystemApp.Data;
using InventorySystemApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly InventoryRepository _repo;

    public AdminController(InventoryRepository repo)
    {
        _repo = repo;
    }

    public IActionResult Dashboard()
    {
        var products = _repo.GetAllProducts();
        return View(products);
    }

    [HttpGet]
    public IActionResult AddProduct() => View();

    [HttpPost]
    public async Task<IActionResult> AddProduct(Product product, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            // Handle image upload for new products
            if (imageFile != null && imageFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/images/" + fileName;
            }

            _repo.AddProduct(product);
            return RedirectToAction("Dashboard");
        }
        return View(product);
    }

    [HttpGet]
    public IActionResult EditProduct(int id)
    {
        var product = _repo.GetProductById(id);
        if (product == null) return NotFound();
        return View(product);
    }

    // This single method now handles both data and the image upload
    [HttpPost]
    public async Task<IActionResult> EditProduct(Product product, IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            // 1. Save new image to wwwroot/images
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // 2. Update the ImageUrl property
            product.ImageUrl = "/images/" + fileName;
        }
        else
        {
            // 3. If no new image is uploaded, keep the old one
            var existingProduct = _repo.GetProductById(product.Id);
            product.ImageUrl = existingProduct?.ImageUrl;
        }

        // 4. Update the database using stock_quantity and image_url
        _repo.UpdateProduct(product);
        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    [Route("Admin/DeleteProduct/{id}")]
    public IActionResult DeleteProduct(int id)
    {
        _repo.DeleteProduct(id);
        return RedirectToAction("Dashboard");
    }

    public IActionResult Orders()
    {
        var orders = _repo.GetAllOrders();
        return View(orders);
    }

    [HttpPost]
    public IActionResult UpdateStatus(int orderID, string status)
    {
        // This handles the 'stock_quantity' logic we fixed earlier
        _repo.UpdateOrderStatus(orderID, status);
        return RedirectToAction("Orders");
    }
}