namespace InventorySystemApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId {  get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime OrderDate { get; set; }

        //Navigation propertise for easy display

        public string  ProductName { get; set; }
        public string CustomerName { get; set; }

        public string Status { get; set; } = "Pending";

     
    }
}
