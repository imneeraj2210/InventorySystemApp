using System.ComponentModel.DataAnnotations;

namespace InventorySystemApp.Models
{
    public class Product
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Product name is required")]
        public string? Name { get; set; }
        [Range(0, 100000, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        [Range(0,100000, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        public string? ImageUrl { get; set; }
    }
}
