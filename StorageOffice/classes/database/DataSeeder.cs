using System;
using System.Linq;
using System.Collections.Generic;
using Bogus;
using StorageOffice.classes.UsersManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace StorageOffice.classes.database;

public class DataSeeder
{
    /// <summary>
    /// Seeds the database with initial data.
    /// This includes creating shops, products, stocks, shippers, shipments, and users.
    /// </summary>
    /// <param name="context">The database context to use for seeding.</param>
    /// <remarks>
    /// This method checks if the database already contains data to avoid duplicate seeding.
    /// It generates random data for shops, products, stocks, shippers, and shipments.
    /// </remarks>
    public static void Seed(StorageContext context)
    {
        // Check if database already contains data to avoid duplicate seeding
        if (context.Shops.Any() || context.Products.Any() || context.Users.Any())
            return;

        var random = new Random();

        // Reset database sequence counters to ensure IDs start at 1
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Shops';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Products';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Stocks';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Shippers';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Shipments';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'ShipmentItems';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Users';");

        // Add users from configuration with appropriate roles
        var users = PasswordManager.GetAllUsers();
        List<User> userList = new List<User>();
        foreach (var user in users)
        {
            userList.Add(new User
            {
                Username = user.Username,
                Role = (UserRole)user.Role
            });
        }
        context.Users.AddRange(userList);
        context.SaveChanges();

        // Generate 10 fictional shops with fake company names and locations
        var shopFaker = new Faker<Shop>()
            .RuleFor(s => s.ShopName, f => f.Company.CompanyName())
            .RuleFor(s => s.Location, f => f.Address.City());

        var shops = shopFaker.Generate(10);
        context.Shops.AddRange(shops);
        context.SaveChanges();

        // Define product categories with realistic items in each category
        var productCategories = new Dictionary<string, List<string>>
        {
            ["Electronics"] = new List<string> 
            { 
            "Smartphone", "Laptop", "Tablet", "Headphones", "Smart Watch", "Power Bank", 
            "Wireless Speaker", "Gaming Console", "Digital Camera", "Bluetooth Earbuds", 
            "External Hard Drive", "Smart Home Hub", "4K Monitor", "VR Headset", "Drone"
            },
            ["Clothing"] = new List<string> 
            { 
            "T-Shirt", "Jeans", "Hoodie", "Sweater", "Dress", "Jacket", "Socks", 
            "Athletic Shoes", "Scarf", "Gloves", "Cap", "Raincoat", "Shorts", 
            "Blazer", "Pajamas", "Swimsuit"
            },
            ["Food"] = new List<string> 
            { 
            "Pasta", "Rice", "Cereal", "Chocolate", "Coffee", "Tea", "Juice", 
            "Frozen Pizza", "Cheese", "Yogurt", "Bread", "Cookies", "Chips", 
            "Peanut Butter", "Jam", "Honey", "Olive Oil", "Spices"
            },
            ["Sporting Goods"] = new List<string> 
            { 
            "Basketball", "Tennis Racket", "Running Shoes", "Yoga Mat", "Dumbbells", 
            "Football", "Baseball Bat", "Golf Clubs", "Hiking Backpack", "Cycling Helmet", 
            "Resistance Bands", "Boxing Gloves", "Skateboard", "Fishing Rod", "Kayak Paddle"
            },
            ["Home Goods"] = new List<string> 
            { 
            "Sofa", "Coffee Table", "Bed Frame", "Lamp", "Kitchen Knife Set", "Cutlery", 
            "Dining Table", "Bookshelf", "Curtains", "Rug", "Wall Clock", "Vacuum Cleaner", 
            "Blender", "Toaster", "Microwave", "Air Purifier", "Laundry Basket"
            }
        };

        // Define appropriate measurement units for each product category
        var units = new Dictionary<string, string>
        {
            ["Electronics"] = "pcs", // pieces
            ["Clothing"] = "pcs",    // pieces
            ["Food"] = "pkg",        // packages
            ["Sporting Goods"] = "pcs", // pieces
            ["Home Goods"] = "pcs"   // pieces
        };

        var products = new List<Product>();

        // Create a balanced set of 20 products across all categories (4 per category)
        int totalProducts = 20;
        int categoriesCount = productCategories.Count;
        int productsPerCategory = totalProducts / categoriesCount;

        // Select random products from each category
        foreach (var category in productCategories)
        {
            var selectedProducts = category.Value.OrderBy(x => Guid.NewGuid()).Take(productsPerCategory).ToList();
            foreach (var productName in selectedProducts)
            {
                var faker = new Faker();
                var product = new Product
                {
                    Name = productName,
                    Category = category.Key,
                    Unit = units[category.Key],
                    Description = $"High-quality {productName.ToLower()} for {GetDescriptionSuffix(category.Key)}"
                };
                products.Add(product);
            }
        }

        // If we didn't reach our target count, add more products randomly
        while (products.Count < totalProducts)
        {
            var randomCategory = productCategories.ElementAt(random.Next(categoriesCount));
            var remainingProducts = randomCategory.Value.Except(products.Select(p => p.Name)).ToList();
            if (remainingProducts.Any())
            {
                var productName = remainingProducts[random.Next(remainingProducts.Count)];
                var product = new Product
                {
                    Name = productName,
                    Category = randomCategory.Key,
                    Unit = units[randomCategory.Key],
                    Description = $"High-quality {productName.ToLower()} for {GetDescriptionSuffix(randomCategory.Key)}"
                };
                products.Add(product);
            }
        }

        context.Products.AddRange(products);
        context.SaveChanges();

        // Create initial stock levels for each product (between 10-99 units)
        foreach (var product in products)
        {
            var stock = new Stock
            {
                Product = product,
                Quantity = random.Next(10, 100),
                LastUpdated = DateTime.Now
            };
            context.Stocks.Add(stock);
        }
        context.SaveChanges();

        // Add common shipping carriers as Shipper entities
        var shipperNames = new List<string> { "FedEx", "UPS", "DHL", "USPS", "Amazon Logistics" };
        var shippers = new List<Shipper>();
        
        foreach (var name in shipperNames)
        {
            var faker = new Faker();
            shippers.Add(new Shipper
            {
                Name = name,
                ContactInfo = faker.Phone.PhoneNumber() + " | " + 
                              $"contact@{name.ToLower().Replace(" ", "")}.com"
            });
        }
        
        context.Shippers.AddRange(shippers);
        context.SaveChanges();

        // Generate shipments - each is either inbound (from a shop) or outbound (to a shipper)
        var shipmentFaker = new Faker<Shipment>()
            .RuleFor(s => s.Shop, f => f.Random.Bool() ? f.PickRandom(shops) : null)
            .RuleFor(s => s.Shipper, (f, s) => s.Shop == null ? f.PickRandom(shippers) : null)
            .RuleFor(s => s.ShipmentType, (f, s) => s.Shop == null ? ShipmentType.Inbound : ShipmentType.Outbound)
            .RuleFor(s => s.User, f => f.PickRandom(userList.Where(u => u.Role == UserRole.Warehouseman)));

        var shipments = shipmentFaker.Generate(30);
        context.Shipments.AddRange(shipments);
        context.SaveChanges();

        // Create shipment items - each shipment contains 1-5 different products
        foreach (var shipment in shipments)
        {
            int itemCount = random.Next(1, 6);
            var selectedProducts = products.OrderBy(x => Guid.NewGuid()).Take(itemCount).ToList();

            foreach (var product in selectedProducts)
            {
                var shipmentItem = new ShipmentItem
                {
                    Shipment = shipment,
                    Product = product,
                    Quantity = random.Next(1, 20) // Each product has 1-19 units in the shipment
                };
                context.ShipmentItems.Add(shipmentItem);
            }
        }
        
        context.SaveChanges();
    }


    /// <summary>
    /// Returns a descriptive suffix based on the product category.
    /// </summary>
    /// <param name="category">The product category for which to generate a suffix.</param>
    /// <returns>A string containing an appropriate description suffix for the specified category,
    /// or a generic suffix if the category is not recognized.</returns>
    private static string GetDescriptionSuffix(string category)
    {
        return category switch
        {
            "Electronics" => "everyday use",
            "Clothing" => "comfortable everyday wear",
            "Food" => "a delicious and nutritious meal",
            "Sporting Goods" => "sports and outdoor activities",
            "Home Goods" => "your home and living spaces",
            _ => "various purposes"
        };
    }
}
