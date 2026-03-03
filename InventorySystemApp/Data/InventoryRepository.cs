using Dapper;
using InventorySystemApp.Models;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Transactions;

namespace InventorySystemApp.Data
{
    public class InventoryRepository
    {
        private readonly string _connectionString;

        public InventoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        // ----------- Authentication Logic -----------

        public User GetUser(string username, string password)
        {
            using var db = CreateConnection();
            string sql = "SELECT * FROM users WHERE username = @username AND password = @password";
            return db.QueryFirstOrDefault<User>(sql, new { username, password });
        }

        // ----------- Product Logic (Admin & User) -----------

        public IEnumerable<Product> GetAllProducts()
{
    using var db = CreateConnection();
    // Use 'AS Stock' to match the property in your Product.cs model
    string sql = "SELECT id, name, price, stock_quantity AS Stock, image_url AS ImageUrl FROM products"; 
    return db.Query<Product>(sql);
}

        public Product GetProductById(int id)
        {
            using var db = CreateConnection();
            // Removed the 'transaction' word from the end of the line
            return db.QueryFirstOrDefault<Product>(
                "SELECT id, name, price, stock_quantity AS Stock FROM products WHERE id = @id",
                new { id });
        }

        public void AddProduct(Product product)
        {
            using var db = CreateConnection();
            string sql = "INSERT INTO products (name, price, stock_quantity) VALUES (@Name, @Price, @Stock)";
            db.Execute(sql, product);
        }

        public void UpdateProduct(Product product)
        {
            using var db = CreateConnection();
            // Use 'stock_quantity' to match your pgAdmin setup
            string sql = @"UPDATE products SET 
                    name = @Name, 
                    price = @Price, 
                    stock_quantity = @Stock, 
                    image_url = @ImageUrl 
                   WHERE id = @Id";
            db.Execute(sql, product);
        }
        public void DeleteProduct(int id)
        {
            using var db = CreateConnection();
            db.Execute("DELETE FROM products WHERE id = @id", new { id });
        }

        // ------------ Order Logic (User) ----------------------

        public bool PlaceOrder(int userId, int productId, int quantity)
        {
            using var db = CreateConnection();
            db.Open();
            using var transaction = db.BeginTransaction();

            try
            {
                // 1. Check Stock (The alias 'AS Stock' maps to Product.cs)
                var product = db.QueryFirstOrDefault<Product>(
                    "SELECT id, name, price, stock_quantity AS Stock FROM products WHERE id = @id",
                    new { id = productId },
                    transaction);

                if (product == null || product.Stock < quantity) return false;

                // 2. Reduce Stock (Matches your DB column 'stock_quantity')
                db.Execute("UPDATE products SET stock_quantity = stock_quantity - @quantity WHERE id = @id",
                    new { quantity, id = productId }, transaction);

                // 3. Record order (Matches your DB columns in image_c8905a)
                string insertSql = @"INSERT INTO orders (user_id, product_id, quantity, order_date, status) 
                             VALUES (@uId, @pId, @qty, @dt, 'Pending')";

                db.Execute(insertSql, new
                {
                    uId = userId,
                    pId = productId,
                    qty = quantity,
                    dt = DateTime.UtcNow
                }, transaction);

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // This will help us see if it's a "Foreign Key" or "Column Name" error
                throw new Exception("DATABASE ERROR: " + ex.Message);
            }
        }

        public IEnumerable<Order> GetAllOrders()
        {
            using var db = CreateConnection();
            string sql = @"SELECT o.id, u.username AS CustomerName, p.name AS ProductName, 
                           o.quantity, o.order_date, o.status 
                           FROM orders o
                           JOIN users u ON o.user_id = u.id
                           JOIN products p ON o.product_id = p.id
                           ORDER BY o.order_date DESC";
            return db.Query<Order>(sql);
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            using var db = CreateConnection();
            db.Open();
            using var trans = db.BeginTransaction();

            try
            {
                db.Execute("UPDATE orders SET status = @status WHERE id = @orderId",
                           new { status, orderId }, trans);

                if (status == "Rejected")
                {
                    var order = db.QueryFirstOrDefault<Order>(
                        "SELECT product_id AS ProductId, quantity AS Quantity FROM orders WHERE id = @orderId",
                        new { orderId }, trans);

                    if (order != null)
                    {
                        // Change 'stock' to 'stock_quantity' to match pgAdmin!
                        db.Execute("UPDATE products SET stock_quantity = stock_quantity + @Quantity WHERE id = @ProductId",
                                   new { order.Quantity, order.ProductId }, trans);
                    }
                }
                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        // Get only the orders for the logged-in user

        public IEnumerable<Order> GetUserOrders(int userId)
        {
            using var db = CreateConnection();
            // 'AS OrderDate' and 'AS Status' ensure the data maps to your C# properties
            string sql = @"SELECT p.name AS ProductName, 
                          o.quantity AS Quantity, 
                          o.order_date AS OrderDate, 
                          o.status AS Status 
                   FROM orders o 
                   JOIN products p ON o.product_id = p.id 
                   WHERE o.user_id = @userId";
            return db.Query<Order>(sql, new { userId });
        }

        // Register User       
        public void RegisterUser(User user)
        {
            using var db = CreateConnection();

            string sql = @"Insert into users (username, password, role)
                            values(@Username, @Password, @Role)";

            db.Execute(sql, user);
        }
    }
}