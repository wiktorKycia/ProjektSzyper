using System;
using System.Linq;
using System.Collections.Generic;
using Bogus;

namespace StorageOffice.classes.database;

public class DataSeeder
{
    public static void Seed(StorageContext context)
    {
        // Check if data already exists
        if (context.Shops.Any() || context.Products.Any())
            return;

        var random = new Random();

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
            ["Electronics"] = new List<string> { "Smartphone", "Laptop", "Tablet", "Headphones", "Smart Watch", "Power Bank", "Wireless Speaker" },
            ["Clothing"] = new List<string> { "T-Shirt", "Jeans", "Hoodie", "Sweater", "Dress", "Jacket", "Socks", "Athletic Shoes" },
            ["Food"] = new List<string> { "Pasta", "Rice", "Cereal", "Chocolate", "Coffee", "Tea", "Juice", "Frozen Pizza" },
            ["Sporting Goods"] = new List<string> { "Basketball", "Tennis Racket", "Running Shoes", "Yoga Mat", "Dumbbells", "Football" },
            ["Home Goods"] = new List<string> { "Sofa", "Coffee Table", "Bed Frame", "Lamp", "Kitchen Knife Set", "Cutlery", "Dining Table" }
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
        int productId = 1;

        // Generate products from each category
        foreach (var category in productCategories)
        {
            foreach (var productName in category.Value)
            {
                var faker = new Faker();
                var product = new Product
                {
                    Name = productName,
                    SKU = faker.Commerce.Ean13(),
                    Category = category.Key,
                    Unit = units[category.Key],
                    Description = $"High-quality {productName.ToLower()} for {GetDescriptionSuffix(category.Key)}"
                };
                products.Add(product);
                
                if (products.Count >= 20) break; // Limit to 20 products total
            }
            if (products.Count >= 20) break;
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

        // Rest of the code remains the same
        var shipmentFaker = new Faker<Shipment>()
            .RuleFor(s => s.Shop, f => f.PickRandom(shops))
            .RuleFor(s => s.Shipper, f => f.Random.Bool(0.8f) ? f.PickRandom(shippers) : null)
            .RuleFor(s => s.ShipmentType, f => f.Random.Enum<ShipmentType>())
            .RuleFor(s => s.ShippedDate, f => f.Date.Past(1));

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
