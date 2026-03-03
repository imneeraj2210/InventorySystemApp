namespace InventorySystemApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        // Update this to match your DB column for clarity
        public int Stock { get; set; }
        public string Status { get; set; }

        public string ImageUrl { get; set; }
    }
}
