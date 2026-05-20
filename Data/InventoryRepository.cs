using Dapper;
using Npgsql;
using InventorySystemApp.Models;

namespace InventorySystemApp.Data
{
    public class InventoryRepository
    {
        private readonly string _connectionString;

        public InventoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection string not found!");
        }

        public IEnumerable<Product> GetAllProduct()
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = "select id, name, price, " +
                          "stock_quantity AS Stock, image_url AS ImageUrl from products";

            return conn.Query<Product>(sql);
        }

        public Product? GetProductById(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = "select id, name, price, " +
                          "stock_quantity AS Stock, " +
                          "image_url AS ImageUrl from products " +
                          "where id = @Id";

            return conn.QueryFirstOrDefault<Product>(sql, new { Id = id });
        }

        public void AddProduct(Product product)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = @"
                        INSERT INTO products(name, price, stock_quantity, image_url)
                        VALUES(@Name, @Price, @Stock, @ImageUrl)";

            conn.Execute(sql, product);
        }

        public void UpdateProduct(Product product)
        {
            using var db = new NpgsqlConnection(_connectionString);

            string sql = @"
        UPDATE products
        SET
            name = @Name,
            price = @Price,
            stock_quantity = @Stock,
            image_url = @ImageUrl
        WHERE id = @Id";

            db.Execute(sql, product);
        }

        public void DeleteProduct(int id)
        {
            using var db = new NpgsqlConnection(_connectionString);

            string sql = "DELETE FROM products WHERE id = @Id";

            db.Execute(sql, new { Id = id });
        }

        public Product? FindProductById(int id)
        {
            using var db = new NpgsqlConnection(_connectionString);

            string sql = @"select id, name, price" +
                        "stock_quantity AS Stock, image_url AS ImageUrl" +
                        "from products where =id = @Id";

            return db.QueryFirstOrDefault<Product>(sql, new { id = id });
        }

        public User? GetUser(string username, string password)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = @"select * from users where " +
                          "username = @Username and " +
                          "password = @Password";

            return conn.QueryFirstOrDefault<User>(sql, new { Username = username, Password = password });
        }

        public void RegisterUser(User user)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = @"Insert into users(username, password, role, email)" +
                        " values(@Username, @Password, @Role, @Email)";

            conn.Execute(sql, user);
        }

        public bool PlaceOrder(int userId, int productId, int quantity)
        {
            using var conn =
                new NpgsqlConnection(_connectionString);

            var product = GetProductById(productId);

            if (product == null ||
                product.Stock < quantity)
                return false;

            string insertSql = @"
        INSERT INTO orders
        (user_id, product_id, quantity, order_date, status)
        VALUES
        (@UserId, @ProductId, @Quantity, NOW(), 'Pending')";

            int rows = conn.Execute(
                insertSql,
                new
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                });

            if (rows > 0)
            {
                string stockSql = @"
            UPDATE products
            SET stock_quantity =
                stock_quantity - @Quantity
            WHERE id = @ProductId";

                conn.Execute(
                    stockSql,
                    new
                    {
                        Quantity = quantity,
                        ProductId = productId
                    });

                return true;
            }

            return false;
        }
        public IEnumerable<Order> GetUserOrders(int userId)
        {
            using var conn =
                new NpgsqlConnection(_connectionString);

            string sql = @"
        SELECT
            o.id,
            o.quantity,
            o.order_date AS OrderDate,
            o.status,
            p.name AS ProductName
        FROM orders o
        INNER JOIN products p
            ON o.product_id = p.id
        WHERE o.user_id = @UserId
        ORDER BY o.id DESC";

            return conn.Query<Order>(
                sql,
                new { UserId = userId });
        }

        public IEnumerable<Order> GetAllOrders()
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = @"
        SELECT
            o.id,
            u.username AS CustomerName,
            p.name AS ProductName,
            o.quantity,
            o.order_date AS OrderDate,
            o.status
        FROM orders o
        JOIN users u
            ON o.user_id = u.id
        JOIN products p
            ON o.product_id = p.id
        ORDER BY o.id DESC";

            return conn.Query<Order>(sql);
        }

        public void CancelOrder(int orderId)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = "Delete from orders where id = @Id";

            conn.Execute(sql, new { Id = orderId });
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = @"Update orders Set status = @Status where id = @OrderId";

            conn.Execute(sql, new { OrderId = orderId, Status = status });
        }

        public User? GetUserByUsername(string username)
        {
            using var conn =
                new NpgsqlConnection(_connectionString);

            string sql =
                @"SELECT id,
                 username,
                 password,
                 role,
                 email
          FROM users
          WHERE username = @Username";

            return conn.QueryFirstOrDefault<User>(
                sql,
                new { Username = username });
        }

        public void AddToCart(int userId, int productId, int quantity)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string checkSql = @"Select id, quantity from cart where user_id = @UserId AND product_id = @ProductId";

            var existing = conn.QueryFirstOrDefault<Cart>(checkSql, new { UserId = userId, ProductId = productId });

            if (existing != null)
            {
                string updateSql = @"Update cart set quantity = quantity + @Quantity where id = @id";

                conn.Execute(updateSql, new { Quantity = quantity, Id = existing.Id });
            }
            else
            {
                string insertSql = @"Insert into cart(user_id, product_id, quantity) values(@UserId, @ProductId, @Quantity)";

                conn.Execute(insertSql, new { UserId = userId, ProductId = productId, Quantity = quantity });
            }

        }

        public IEnumerable<Cart> GetCartItems(int userId)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = @"SELECT c.id,
            c.user_id AS UserId,
            c.product_id AS ProductId,
            c.quantity,
            p.name AS ProductName,
            p.price,
            p.image_url AS ImageUrl
          FROM cart c
          JOIN products p
            ON c.product_id = p.id
          WHERE c.user_id=@UserId";

            return conn.Query<Cart>(sql, new { UserId = userId });
        }

        public void RemoveCartItem(int cartId)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = @"DELETE FROM cart WHERE id=@Id";

            conn.Execute(sql, new { Id = cartId });
        }

        public void UpdateCartQuantity(int cartId, int quantity)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = @"Update cart set quantity = @Quantity where id = @Id";

            conn.Execute(sql, new { Quantity = quantity, Id = cartId });
        }

        public int GetCartCount(int userId)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = "select count(*) from cart where user_id = @UserId";

            return conn.ExecuteScalar<int>(sql, new { UserId = userId });
        }

        public void CheckoutCart(int userId)
        {
            using var conn =
                new NpgsqlConnection(
                    _connectionString);

            conn.Open();

            using var tran =
                conn.BeginTransaction();

            var cartItems =
                conn.Query<Cart>(
                    @"SELECT
                product_id AS ProductId,
                quantity
              FROM cart
              WHERE user_id=@UserId",

                    new
                    {
                        UserId = userId
                    },
                    tran);

            foreach (var item in cartItems)
            {
                conn.Execute(
                    @"INSERT INTO orders
              (
                user_id,
                product_id,
                quantity,
                order_date,
                status
              )
              VALUES
              (
                @UserId,
                @ProductId,
                @Quantity,
                NOW(),
                'Pending'
              )",

                    new
                    {
                        UserId = userId,
                        item.ProductId,
                        item.Quantity
                    },
                    tran);

                conn.Execute(
                    @"UPDATE products
              SET stock_quantity =
                  stock_quantity - @Quantity
              WHERE id=@ProductId",

                    new
                    {
                        item.Quantity,
                        item.ProductId
                    },
                    tran);
            }

            conn.Execute(
                @"DELETE
          FROM cart
          WHERE user_id=@UserId",

                new
                {
                    UserId = userId
                },
                tran);

            tran.Commit();
        }

        public IEnumerable<Product> SearchProducts(string searchTerm)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = "Select id, name, price, stock_quantity AS Stock, image_url AS ImageUrl from products where Lower(name) like Lower(@Search)";

            return conn.Query<Product>(sql, new { Search = "%"+searchTerm+"%" });

        }

        public IEnumerable<Product> GetProductsPaged(int page, int pageSize)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            int offset = (page - 1) * pageSize;

            string sql = @"Select id, name, price, stock_quantity AS Stock, image_url As ImageUrl from products Order By id Limit @PageSize OFFSET @Offset";

            return conn.Query<Product>(sql, new { PageSize = pageSize, Offset = offset });
        }

        public int GetProductCount()
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = @"Select COUNT(*) from products";

            return conn.ExecuteScalar<int>(sql);
        }

        public IEnumerable<Product> SortProducts(string sortOrder)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = @"select id, name, price, stock_quantity AS Stock, image_url AS ImageUrl from products ";

            switch(sortOrder)
            {
                case "price_asc":
                    sql += "Order By price ASC";
                    break;

                case "price_desc":
                    sql += "Order By price DESC";
                    break;

                case "stock":
                    sql += "Order By stock_quantity DESC";
                    break;

                default:
                    sql += "Order by id DESC";
                    break;
            }

            return conn.Query<Product>(sql);
        }

        public IEnumerable<Product> FilterProducts(string stockFilter)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = "select id, name, price, stock_quantity AS Stock, image_url As ImageUrl from products";
            
            switch(stockFilter)
            {
                case "instock":
                    sql += " where stock_quantity > 0";
                    break;

                case "outofstock":
                    sql += " where stock_quantity = 0";
                    break;

                case "lowstock":
                    sql += " where stock_quantity < 10";
                    break;

                default:
                    sql += " Order by id DESC";
                    break;                           
    
            }

            return conn.Query<Product>(sql);
        }

        public WebsiteSetting GetWebsiteSettings()
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = @"Select id, site_name AS SiteName, logo_url AS LogoUrl from website_settings limit 1";

            return conn.QueryFirstOrDefault<WebsiteSetting>(sql)!;
        }

        public void UpdateWebsiteSettings(WebsiteSetting setting)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            string sql = "Update website_settings set site_name = @SiteName, logo_url=@LogoUrl where id = @Id";

            conn.Execute(sql, setting);
        }

        public int GetTotalProducts()
        {
            using var conn =
                new NpgsqlConnection(
                    _connectionString);

            string sql =
                "SELECT COUNT(*) FROM products";

            return conn.ExecuteScalar<int>(
                sql);
        }

        public int GetTotalUsers()
        {
            using var conn =
                new NpgsqlConnection(
                    _connectionString);

            string sql =
                "SELECT COUNT(*) FROM users";

            return conn.ExecuteScalar<int>(
                sql);
        }

        public int GetTotalOrders()
        {
            using var conn =
                new NpgsqlConnection(
                    _connectionString);

            string sql =
                "SELECT COUNT(*) FROM orders";

            return conn.ExecuteScalar<int>(
                sql);
        }

        public int GetPendingOrders()
        {
            using var conn =
                new NpgsqlConnection(
                    _connectionString);

            string sql =
                @"SELECT COUNT(*)
          FROM orders
          WHERE status='Pending'";

            return conn.ExecuteScalar<int>(
                sql);
        }
    }


}
