namespace InventorySystemApp.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public DateTime OrderDate { get; set; }

        public string? Status { get; set; }

        public string? ProductName { get; set; }

        public string? CustomerName { get; set; }
    }
}