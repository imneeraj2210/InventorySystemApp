using InventorySystemApp.Data;
using InventorySystemApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystemApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly InventoryRepository _repo;

        public HomeController(InventoryRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index(string? searchTerm, string? sortOrder, string? stockFilter, int page = 1)
        {
            int pageSize = 4;

            IEnumerable<Product> products;

            int totalProducts = 0;
            
            //Search
            if (!string.IsNullOrWhiteSpace(
                searchTerm))
            {
                products =
                    _repo.SearchProducts(
                        searchTerm);

                totalProducts =
                    products.Count();
            }
            //Filter
            else if(!string.IsNullOrWhiteSpace(stockFilter))
            {
                products = _repo.FilterProducts(stockFilter);

                totalProducts = products.Count();
            }

            //Sorting
            else if(!string.IsNullOrWhiteSpace(sortOrder))
            {
                products = _repo.SortProducts(sortOrder);

                totalProducts += products.Count();
            }
            //Pagination
            else
            {
                products =
                    _repo.GetProductsPaged(
                        page,
                        pageSize);

                totalProducts =
                    _repo.GetProductCount();
            }

            ViewBag.SearchTerm =
                searchTerm;

            ViewBag.CurrentPage =
                page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    (double)totalProducts /
                    pageSize);

            return View(products);
        }
    }
}
