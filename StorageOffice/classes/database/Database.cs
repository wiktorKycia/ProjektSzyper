using Bogus.DataSets;
using Microsoft.EntityFrameworkCore;
using StorageOffice.classes.LogServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageOffice.classes.database
{
    /// <summary>
    /// This class allows to use the storage database and contains methods that allow to perform all actions on the tables such as editing records, adding new ones or deleting them.
    /// </summary>
    public class StorageDatabase
    {
        private StorageContext _context = new();
        public string Path { get { return _context.DbPath; } }
        private event Action<string> _storageDataChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageDatabase"/> class. Sets appropriate protection for the correct operation of the database and assigns appropriate logging method to event.
        /// </summary>
        public StorageDatabase()
        {
            _context.Database.SetCommandTimeout(5);
            _context.Database.Migrate();
            _storageDataChanged += (action) => LogManager.AddNewLog($"Info: action was performed on data in database - {action}");
        }

        // Stores management methods
        /// <summary>
        /// Allows to add a new store to the database. This method validates the data, makes sure that there is no such store in the database and adds appropriate log.
        /// </summary>
        /// <param name="shopName">String representing the store name.</param>
        /// <param name="location">String representing the store location.</param>
        /// <exception cref="InvalidOperationException">This exception is thrown when there is an attempt to add a store with a name that is already taken.</exception>
        public void AddShop(string? shopName, string? location)
        {
            Shop.Validate(shopName, location);
            if (_context.Shops.Any(s => s.ShopName.ToLower() == shopName!.ToLower()))
            {
                throw new InvalidOperationException("A shop with this name already exists!");
            }
            Shop shop = new Shop() { ShopName = shopName!, Location = location!};
            _context.Shops.Add(shop);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"added shop named '{shopName}'");
        }

        /// <summary>
        /// Updates the data of a store with a specific id in the database. If a parameter is set to null, it means that it is not to be changed in the database. Adds appropriate log about changes.
        /// </summary>
        /// <param name="shopId">Integer representing the id of the store whose data is to be changed.</param>
        /// <param name="shopName">String representing the new store name or null.</param>
        /// <param name="location">String representing the store new location or null.</param>
        /// <exception cref="InvalidOperationException">This exception is thrown when there is an attempt to modify a store's name to a name that is already taken.</exception>
        public void UpdateShop(int shopId, string? shopName, string? location)
        {
            Shop shop = GetShopById(shopId);
            if (!string.IsNullOrEmpty(shopName))
            {
                if (_context.Shops.Any(s => s.ShopName.ToLower() == shopName.ToLower()))
                {
                    throw new InvalidOperationException("A shop with this name already exists!");
                }
                shop.ShopName = shopName;
            }
            if (!string.IsNullOrEmpty(location))
            {
                shop.Location = location;
            }
            _context.SaveChanges();
            _storageDataChanged?.Invoke("modified shop's data");
        }

        /// <summary>
        /// Deletes a shop from database. Adds appropriate log about the removal.
        /// </summary>
        /// <param name="shopId">Integer representing the id of the store that has to be deleted.</param>
        public void DeleteShop(int shopId)
        {
            Shop shop = GetShopById(shopId);
            string shopName = shop.ShopName;
            _context.Shops.Remove(shop);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"removed shop named {shopName}");
        }

        /// <summary>
        /// Allows to get a list of all shops from the database.
        /// </summary>
        /// <returns>List of Shop objects from the database.</returns>
        public List<Shop> GetAllShops() => [.. _context.Shops];

        /// <summary>
        /// Allows to get particular shop from the database by it's id.
        /// </summary>
        /// <param name="shopID">Integer representing the id of the shop that has to be returned.</param>
        /// <returns>Shop object with the given id found in the database.</returns>
        /// <exception cref="InvalidOperationException">This exception is returned when a store with the given id does not exist in the database.</exception>
        public Shop GetShopById(int shopID)
        {
            Shop? shop = _context.Shops.Find(shopID);
            if (shop != null)
            {
                return shop;
            }
            throw new InvalidOperationException("The given shop's id doesn't exist in database!");
        }

        // Products and Stocks management methods
        /// <summary>
        /// Allows to add a new product and it's stock to the database. This method validates the data, makes sure that there is no such product in the database and adds an appropriate log.
        /// </summary>
        /// <param name="name">String representing the new product name.</param>
        /// <param name="category">String representing the new product's category name.</param>
        /// <param name="unit">String representing the new product's unit.</param>
        /// <param name="description">String representing the new product's description.</param>
        /// <exception cref="InvalidOperationException">This exception is thrown when there is an attempt to add a product with a name that is already taken.</exception>
        public void AddProductAndStock(string? name, string? category, string? unit, string? description)
        {
            Product.Validate(name, category, unit, description);
            if (_context.Products.Any(p => p.Name.ToLower() == name!.ToLower()))
            {
                throw new InvalidOperationException("A product with this name already exists!");
            }
            Product product = new Product() { Name = name!, Category = category!, Unit = unit!, Description = description!, Stock = new Stock() { Quantity = 0, LastUpdated = DateTime.Now } };
            _context.Products.Add(product);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"added product '{name}' and it's stock");
        }

        // Products management methods
        /// <summary>
        /// Updates the data of a product with a specific id in the database. If a parameter is set to null, it means that it is not to be changed in the database. Adds appropriate log about changes.
        /// </summary>
        /// <param name="productId">Integer representing the id of the product whose data is to be changed.</param>
        /// <param name="name">String representing the new product's name or null.</param>
        /// <param name="category">String representing the new product's category name or null.</param>
        /// <param name="unit">String representing the new product's unit or null.</param>
        /// <param name="description">String representing the new product's description or null.</param>
        /// <exception cref="InvalidOperationException">This exception is thrown when there is an attempt to modify a product's name to a name that is already taken.</exception>
        public void UpdateProduct(int productId, string? name, string? category, string? unit, string? description)
        {
            Product product = GetProductById(productId);
            if (!string.IsNullOrEmpty(name))
            {
                if (_context.Products.Any(p => p.Name.ToLower() == name.ToLower()))
                {
                    throw new InvalidOperationException("A product with this name already exists!");
                }
                product.Name = name;
            }
            if (!string.IsNullOrEmpty(category))
            {
                product.Category = category;
            }
            if (!string.IsNullOrEmpty(unit))
            {
                product.Unit = unit;
            }
            if (!string.IsNullOrEmpty(description))
            {
                product.Description = description;
            }
            _context.SaveChanges();
            _storageDataChanged?.Invoke("modified product's data");
        }

        /// <summary>
        /// Deletes a product from database. Adds appropriate log about the removal.
        /// </summary>
        /// <param name="productId">Integer representing the id of the product that has to be deleted.</param>
        public void DeleteProduct(int productId)
        {
            Product product = GetProductById(productId);
            string productName = product.Name;
            _context.Products.Remove(product);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"removed product named {productName}");
        }

        /// <summary>
        /// Allows to get a list of all products and their stocks from the database.
        /// </summary>
        /// <returns>List of Product and it's Stock objects from the database.</returns>
        public List<Product> GetAllProducts() => [.. _context.Products.Include(p => p.Stock)];

        /// <summary>
        /// Allows to get a list of all products with specific name and their stocks from the database.
        /// </summary>
        /// <param name="name">String representing the name of the products that should be returned.</param>
        /// <returns>List of appropriate Product and it's Stock objects from the database.</returns>
        public List<Product> GetAllproductsByName(string name) => _context.Products.Where(p => p.Name == name).Include(p => p.Stock).ToList();

        /// <summary>
        /// Allows to get a list of all products from specific category and their stocks from the database.
        /// </summary>
        /// <param name="category">String representing the category of the products that should be returned.</param>
        /// <returns>List of appropriate Product and it's Stock objects from the database.</returns>
        public List<Product> GetAllproductsByCategory(string category) => _context.Products.Where(p => p.Category == category).Include(p => p.Stock).ToList();

        /// <summary>
        /// Allows to get particular product from the database by it's id.
        /// </summary>
        /// <param name="productId">Integer representing the id of the product that has to be returned.</param>
        /// <returns>Product object with the given id found in the database.</returns>
        /// <exception cref="InvalidOperationException">This exception is returned when a product with the given id does not exist in the database.</exception>
        public Product GetProductById(int productId)
        {
            Product? product = _context.Products.Find(productId);
            if (product != null)
            {
                return product;
            }
            throw new InvalidOperationException("The given product's id doesn't exist in database!");
        }

        // Stocks management methods
        /// <summary>
        /// Updates the data of a stock with a specific id in the database. If a parameter is set to null, it means that it is not to be changed in the database. Adds appropriate log about changes.
        /// </summary>
        /// <param name="stockId">Integer representing the id of the stock whose data is to be changed.</param>
        /// <param name="quantity">Integer representing the new stock's quantity of a product or null.</param>
        public void UpdateStock(int stockId, int quantity)
        {
            Stock stock = GetStockByID(stockId);
            DateTime newLastUpdated = DateTime.Now;
            Stock.Validate(quantity, newLastUpdated);
            stock.Quantity = quantity;
            stock.LastUpdated = newLastUpdated;
            _context.SaveChanges();
            _storageDataChanged?.Invoke("modified stock's data");
        }

        /// <summary>
        /// Deletes a stock from database. Adds appropriate log about the removal.
        /// </summary>
        /// <param name="stockID">Integer representing the id of the stock that has to be deleted.</param>
        public void DeleteStock(int stockID)
        {
            Stock stock = GetStockByID(stockID);
            _context.Stocks.Remove(stock);
            _context.SaveChanges();
            _storageDataChanged?.Invoke("removed product's stock");
        }

        /// <summary>
        /// Allows to get a list of all stocks from the database.
        /// </summary>
        /// <returns>List of Stock objects from the database.</returns>
        public List<Stock> GetAllStocks() => [.. _context.Stocks];

        /// <summary>
        /// Allows to get particular stock from the database by it's id.
        /// </summary>
        /// <param name="stockId">Integer representing the id of the stock that has to be returned.</param>
        /// <returns>Stock object with the given id found in the database.</returns>
        /// <exception cref="InvalidOperationException">This exception is returned when a stock with the given id does not exist in the database.</exception>
        public Stock GetStockByID(int stockId)
        {
            Stock? stock = _context.Stocks.Find(stockId);
            if (stock != null)
            {
                return stock;
            }
            throw new InvalidOperationException("The given stock's id doesn't exist in database!");
        }

        // Shippers management methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contactInfo"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddShipper(string? name, string? contactInfo)
        {
            Shipper.Validate(name, contactInfo);
            if (_context.Shippers.Any(s => s.Name.ToLower() == name!.ToLower()))
            {
                throw new InvalidOperationException("A shipper with this name already exists!");
            }
            Shipper shipper = new Shipper() { Name = name!, ContactInfo = contactInfo!};
            _context.Shippers.Add(shipper);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"added shipper named '{name}'");
        }

        public void UpdateShipper(int shipperId, string? name, string? contactInfo)
        {
            Shipper shipper = GetShipperById(shipperId);
            if (!string.IsNullOrEmpty(name))
            {
                if (_context.Shippers.Any(s => s.Name.ToLower() == name.ToLower()))
                {
                    throw new InvalidOperationException("A shipper with this name already exists!");
                }
                shipper.Name = name;
            }
            if (!string.IsNullOrEmpty(contactInfo))
            {
                shipper.ContactInfo = contactInfo;
            }
            _context.SaveChanges();
            _storageDataChanged?.Invoke("modified shippers data");
        }

        public void DeleteShipper(int shipperId)
        {
            Shipper shipper = GetShipperById(shipperId);
            string shipperName = shipper.Name;
            _context.Shippers.Remove(shipper);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"removed shipper named '{shipperName}'");
        }

        public List<Shipper> GetAllShippers() => [.. _context.Shippers];

        public Shipper GetShipperById(int shipperId)
        {
            Shipper? shipper = _context.Shippers.Find(shipperId);
            if (shipper != null)
            {
                return shipper;
            }
            throw new InvalidOperationException("The given shipper's id doesn't exist in database!");
        }

        // Shipments management methods
        public void AddInboundShipment(int shipperId)
        {
            Shipper shipper = GetShipperById(shipperId);
            Shipment shipment = new Shipment() { ShipmentType = ShipmentType.Inbound, Shipper = shipper, IsCompleted = false};
            _context.Shipments.Add(shipment);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"added new inbound shipment from {shipper.Name}");
        }

        public void AddOutboundShipment(int shopId)
        {
            Shop shop = GetShopById(shopId);
            Shipment shipment = new Shipment() { ShipmentType = ShipmentType.Outbound, Shop = shop, IsCompleted = false };
            _context.Shipments.Add(shipment);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"added new outbound shipment to {shop.ShopName}");
        }

        public void UpdateShipmentType(int shipmentId, int? shopId, int? shipperId, string? shipmentType)
        {
            Shipment shipment = GetShipmentById(shipmentId);
            Shipment.Validate(shipmentType);
            ShipmentType newShipmentType;
            Enum.TryParse(shipmentType, out newShipmentType);
            if(newShipmentType == ShipmentType.Inbound)
            {
                if (shipperId != null)
                {
                    Shipper shipper = GetShipperById((int)shipperId);
                    shipment.Shipper = shipper;
                }
                else
                {
                    throw new InvalidOperationException("The inbound shipment must have a shipper assigned!");
                }
            }
            else
            {
                if (shopId != null)
                {
                    Shop shop = GetShopById((int)shopId);
                    shipment.Shop = shop;
                }
                else
                {
                    throw new InvalidOperationException("The outbound shipment must have a shop assigned!");
                }
            }
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"modified shipment type to {newShipmentType}");
        }

        public void UpdateUserAssaignedToShipment(int userId, int shipmentId)
        {
            User user = GetUserById(userId);
            Shipment shipment = GetShipmentById(shipmentId);
            shipment.User = user;
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"modified user assigned to shipment to {user.Username}");
        }

        public void MarkShipmentAsDone(int shipmentId)
        {
            Shipment shipment = GetShipmentById(shipmentId);
            if (shipment.ShipmentType == ShipmentType.Inbound)
            {
                foreach (ShipmentItem shipmentItem in shipment.ShipmentItems)
                {
                    Stock stock = _context.Stocks.Single(s => s.Product == shipmentItem.Product);
                    UpdateStock(stock.StockId, stock.Quantity + shipmentItem.Quantity);
                }
            }
            else
            {
                foreach (ShipmentItem shipmentItem in shipment.ShipmentItems)
                {
                    Stock stock = _context.Stocks.Single(s => s.Product == shipmentItem.Product);
                    Stock.Validate(stock.Quantity - shipmentItem.Quantity);
                    UpdateStock(stock.StockId, stock.Quantity - shipmentItem.Quantity);
                }
            }
            shipment.IsCompleted = true;
            shipment.ShippedDate = DateTime.Now;
            _context.SaveChanges();
            _storageDataChanged?.Invoke("marked shipment as done");
        }

        public void DeleteShipment(int shipmentId)
        {
            Shipment shipment = GetShipmentById(shipmentId);
            _context.Shipments.Remove(shipment);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"removed shipment with id #{shipmentId}");
        }

        public List<Shipment> GetAllInboundShipments() => [.. _context.Shipments.Where(s => s.ShipmentType == ShipmentType.Inbound).Include(s => s.Shipper).Include(s => s.ShipmentItems).ThenInclude(si => si.Product)];

        public List<Shipment> GetAllOutboundShipments() => [.. _context.Shipments.Where(s => s.ShipmentType == ShipmentType.Outbound).Include(s => s.Shop).Include(s => s.ShipmentItems).ThenInclude(si => si.Product)];

        public List<Shipment> GetAllNotCompletedInboundShipments() => [.. _context.Shipments.Where(s => s.ShipmentType == ShipmentType.Inbound && s.IsCompleted == false).Include(s => s.Shipper).Include(s => s.ShipmentItems).ThenInclude(si => si.Product)];

        public List<Shipment> GetAllNotCompletedOutboundShipments() => [.. _context.Shipments.Where(s => s.ShipmentType == ShipmentType.Outbound && s.IsCompleted == false).Include(s => s.Shop).Include(s => s.ShipmentItems).ThenInclude(si => si.Product)];

        public List<Shipment> GetAllShipments() => GetAllInboundShipments()
            .Concat(GetAllOutboundShipments())
            .OrderBy(s => s.ShipmentId)
            .ToList();

        public List<Shipment> GetNotCompletedShipmentsAssignedToUser(int userId)
        {
            User user = GetUserById(userId);
            return GetAllShipments().Where(s => s.User == user && s.IsCompleted == false).ToList();
        }

        public List<Shipment> GetNotCompletedShipments() => GetAllNotCompletedInboundShipments().Concat(GetAllNotCompletedOutboundShipments()).ToList();

        public Shipment GetShipmentById(int shipmentId)
        {
            Shipment? shipment = _context.Shipments.Find(shipmentId);
            if (shipment != null)
            {
                return shipment;
            }
            throw new InvalidOperationException("The given shipment's id doesn't exist in database!");
        }

        // ShipmentItems management methods
        public void AddShipmentItem(int quantity, int shipmentId, int productId)
        {
            ShipmentItem.Validate(quantity);
            Shipment shipment = GetShipmentById(shipmentId);
            Product product = GetProductById(productId);
            if(_context.ShipmentItems.Any(s => s.Shipment == shipment && s.Product == product))
            {
                throw new InvalidOperationException("The given shipment item is already added to this shipment!");
            }

            ShipmentItem shipmentItem = new ShipmentItem() { Quantity = quantity, Product = product, Shipment = shipment};
            _context.ShipmentItems.Add(shipmentItem);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"added product '{product.Name}' to shipment with id #{shipmentId}");
        }

        public void UpdateShipmentItem(int shipmentItemId, int quantity)
        {
            ShipmentItem.Validate(quantity);
            ShipmentItem shipmentItem = GetShipmentItemById(shipmentItemId);
            shipmentItem.Quantity = quantity;
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"modified item's quantity in shipment");
        }

        public void DeleteShipmentItem(int shipmentItemId)
        {
            ShipmentItem shipmentItem = GetShipmentItemById(shipmentItemId);
            _context.ShipmentItems.Remove(shipmentItem);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"removed shipment item from shipment");
        }

        public List<ShipmentItem> GetAllShipmentItems() => [.. _context.ShipmentItems];

        public ShipmentItem GetShipmentItemById(int shipmentItemId)
        {
            ShipmentItem? shipmentItem = _context.ShipmentItems.Find(shipmentItemId);
            if (shipmentItem != null)
            {
                return shipmentItem;
            }
            throw new InvalidOperationException("The given shipment's item id doesn't exist in database!");
        }

        // Users management methods
        public void AddUser(string? username, string? role)
        {
            User.Validate(username, role);
            if (_context.Users.Any(s => s.Username.ToLower() == username!.ToLower()))
            {
                throw new InvalidOperationException("A user with this username already exists!");
            }
            UserRole userRole;
            Enum.TryParse(role, out userRole);
            User user = new User() { Username = username!, Role = userRole};
            _context.Users.Add(user);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"added user named 'username'");
        }

        public void UpdateUser(int userId, string? username, string? role)
        {
            User user = GetUserById(userId);
            if (!string.IsNullOrEmpty(username))
            {
                if (_context.Users.Any(s => s.Username.ToLower() == username.ToLower()))
                {
                    throw new InvalidOperationException("A user with this username already exists!");
                }
                user.Username = username;
            }
            if (!string.IsNullOrEmpty(role))
            {
                UserRole userRole;
                Enum.TryParse(role, out userRole);
                user.Role = userRole;
            }
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"modified user's data");
        }

        public void DeleteUser(int userID)
        {
            User user = GetUserById(userID);
            string username = user.Username;
            _context.Users.Remove(user);
            _context.SaveChanges();
            _storageDataChanged?.Invoke($"removed user named '{username}'");
        }
        
        public List<User> GetAllUsers() => [.. _context.Users];

        public User GetUserById(int userId)
        {
            User? user = _context.Users.Find(userId);
            if (user != null)
            {
                return user;
            }
            throw new InvalidOperationException("The given user's id doesn't exist in database!");
        }

        public User GetUserByUsername(string username)
        {
            User? user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user != null)
            {
                return user;
            }
            throw new InvalidOperationException("The given user's username doesn't exist in database!");
        }
        public int GetUserIdByUsername(string username)
        {
            User? user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user != null)
            {
                return user.UserId;
            }
            throw new InvalidOperationException("The given user's username doesn't exist in database!");
        }

        public void SeedData() => DataSeeder.Seed(_context);

        public void ClearDatabase()
        {
            _context.RemoveRange(_context.Shops);
            _context.RemoveRange(_context.Products);
            _context.RemoveRange(_context.Stocks);
            _context.RemoveRange(_context.Shippers);
            _context.RemoveRange(_context.Shipments);
            _context.RemoveRange(_context.ShipmentItems);
            _context.RemoveRange(_context.Users);
            _context.SaveChanges();
        }
    }
}
