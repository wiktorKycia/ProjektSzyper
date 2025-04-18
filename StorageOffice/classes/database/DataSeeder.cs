using System;
using System.Linq;
using System.Collections.Generic;
using Bogus;
using StorageOffice.classes.UsersManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace StorageOffice.classes.database;

public class DataSeeder
{
    public static void Seed(StorageContext context)
    {
        // Check if data already exists
        if (context.Shops.Any() || context.Products.Any() || context.Users.Any())
            return;

        var random = new Random();

        // Set the starting value for each table's primary key to 1
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Shops';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Products';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Stocks';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Shippers';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Shipments';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'ShipmentItems';");
        context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Users';");

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

        // Generate fake Shops
        var shopFaker = new Faker<Shop>()
            .RuleFor(s => s.ShopName, f => f.Company.CompanyName())
            .RuleFor(s => s.Location, f => f.Address.City());

        var shops = shopFaker.Generate(10);
        context.Shops.AddRange(shops);
        context.SaveChanges();

        // Create realistic product categories with appropriate products
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

        var units = new Dictionary<string, string>
        {
            ["Electronics"] = "pcs",
            ["Clothing"] = "pcs",
            ["Food"] = "pkg",
            ["Sporting Goods"] = "pcs",
            ["Home Goods"] = "pcs"
        };

        var products = new List<Product>();

        // Generate products from each category
        int totalProducts = 20; // Limit to 20 products total
        int categoriesCount = productCategories.Count;
        int productsPerCategory = totalProducts / categoriesCount;

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

        // If there are fewer than totalProducts, fill the remaining slots randomly
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

        // Generate fake Stock for each product
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

        // Generate fake Shippers
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

        // Ensure that each shipment has either a Shop or a Shipper, but not both
        var shipmentFaker = new Faker<Shipment>()
            .RuleFor(s => s.Shop, f => f.Random.Bool() ? f.PickRandom(shops) : null)
            .RuleFor(s => s.Shipper, (f, s) => s.Shop == null ? f.PickRandom(shippers) : null)
            .RuleFor(s => s.ShipmentType, (f, s) => s.Shop == null ? ShipmentType.Outbound : ShipmentType.Inbound)
            .RuleFor(s => s.ShippedDate, f => f.Date.Past(1))
            .RuleFor(s => s.User, f => f.PickRandom(userList.Where(u => u.Role == UserRole.Warehouseman)));

        var shipments = shipmentFaker.Generate(30);
        context.Shipments.AddRange(shipments);
        context.SaveChanges();

        // Generate fake ShipmentItems
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
                    Quantity = random.Next(1, 20)
                };
                context.ShipmentItems.Add(shipmentItem);
            }
        }
        
        context.SaveChanges();
    }

    private static string GetDescriptionSuffix(string category)
    {
        return category switch
        {
            "Electronics" => "everyday use with the latest technology",
            "Clothing" => "comfortable everyday wear",
            "Food" => "a delicious and nutritious meal",
            "Sporting Goods" => "sports and outdoor activities",
            "Home Goods" => "your home and living spaces",
            _ => "various purposes"
        };
    }
}
