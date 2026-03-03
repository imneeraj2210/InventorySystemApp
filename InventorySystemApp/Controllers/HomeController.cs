using InventorySystemApp.Data;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly InventoryRepository _repo; // Define the repository

    public HomeController(InventoryRepository repo) // Inject it through the constructor
    {
        _repo = repo;
    }

    public IActionResult Index()
    {
        var products = _repo.GetAllProducts();
        return View(products);
    }
}