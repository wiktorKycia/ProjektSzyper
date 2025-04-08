using System;
using System.Linq;
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

        // Generate fake Products
        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.SKU, f => f.Commerce.Ean13())
            .RuleFor(p => p.Category, f => f.Commerce.Department())
            .RuleFor(p => p.Unit, f => "pcs")
            .RuleFor(p => p.Description, f => f.Lorem.Sentence());

        var products = productFaker.Generate(20);
        context.Products.AddRange(products);

        // Generate fake Stock for each product
        foreach (var product in products)
        {
            var stock = new Stock
            {
                ProductId = product.ProductId,
                Quantity = random.Next(10, 100),
                LastUpdated = DateTime.Now
            };
            context.Stocks.Add(stock);
        }

        // Generate fake Shippers
        var shipperFaker = new Faker<Shipper>()
            .RuleFor(s => s.Name, f => f.Company.CompanyName())
            .RuleFor(s => s.ContactInfo, f => f.Phone.PhoneNumber() + " | " + f.Internet.Email());

        var shippers = shipperFaker.Generate(5);
        context.Shippers.AddRange(shippers);

        // Generate fake Shipments
        var shipmentFaker = new Faker<Shipment>()
            .RuleFor(s => s.ShopId, f => f.PickRandom(shops).ShopId)
            .RuleFor(s => s.ShipperId, f => f.Random.Bool(0.8f) ? f.PickRandom(shippers).ShipperId : (int?)null) // 80% have a shipper
            .RuleFor(s => s.ShipmentType, f => f.Random.Enum<ShipmentType>())
            .RuleFor(s => s.ShippedDate, f => f.Date.Past(1));

        var shipments = shipmentFaker.Generate(30);
        context.Shipments.AddRange(shipments);

        // Generate fake ShipmentItems
        foreach (var shipment in shipments)
        {
            // Each shipment has 1-5 different products
            int itemCount = random.Next(1, 6);
            var selectedProducts = products.OrderBy(x => Guid.NewGuid()).Take(itemCount).ToList();

            foreach (var product in selectedProducts)
            {
                var shipmentItem = new ShipmentItem
                {
                    ShipmentId = shipment.ShipmentId,
                    ProductId = product.ProductId,
                    Quantity = random.Next(1, 20)
                };
                context.ShipmentItems.Add(shipmentItem);
            }
        }

        // Save all changes to the database
        context.SaveChanges();
    }
}
