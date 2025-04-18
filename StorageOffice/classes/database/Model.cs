using Bogus.Bson;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace StorageOffice.classes.database;

public class StorageContext : DbContext
{
    public DbSet<Shop> Shops { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Shipper> Shippers { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<ShipmentItem> ShipmentItems { get; set; }
    public DbSet<User> Users { get; set; } 

    public string DbPath { get; }

    public StorageContext(string? customDbPath = null)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;

        // Create app folder in AppData/Local (MS Windows) if it doesn't exist yet
        var appPath = Path.Join(Environment.GetFolderPath(folder), "StorageOffice");
        Directory.CreateDirectory(appPath);

        this.DbPath = customDbPath ?? Path.Join(appPath, "StorageOffice.db");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}", opt =>
        {
            opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One-to-one relationship between Product and Stock.
        modelBuilder.Entity<Product>()
                    .HasOne(p => p.Stock)
                    .WithOne(s => s.Product)
                    .HasForeignKey<Stock>(s => s.ProductId);

        // One-to-many: Shop -> Shipments.
        modelBuilder.Entity<Shop>()
                    .HasMany(s => s.Shipments)
                    .WithOne(sh => sh.Shop)
                    .HasForeignKey(sh => sh.ShopId);

        // One-to-many: Shipper -> Shipments.
        modelBuilder.Entity<Shipper>()
                    .HasMany(s => s.Shipments)
                    .WithOne(sh => sh.Shipper)
                    .HasForeignKey(sh => sh.ShipperId)
                    .OnDelete(DeleteBehavior.SetNull); // If a shipper is removed, set FK to null.

        // One-to-many: Shipment -> ShipmentItems.
        modelBuilder.Entity<Shipment>()
                    .HasMany(s => s.ShipmentItems)
                    .WithOne(si => si.Shipment)
                    .HasForeignKey(si => si.ShipmentId);

        // One-to-many: Product -> ShipmentItems.
        modelBuilder.Entity<Product>()
                    .HasMany(p => p.ShipmentItems)
                    .WithOne(si => si.Product)
                    .HasForeignKey(si => si.ProductId);
        
        // One-to-many: User -> Shipments.
        modelBuilder.Entity<User>()
                    .HasMany(u => u.Shipments)
                    .WithOne(s => s.User)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.SetNull); // If a user is removed, set FK to null.
    }
}

// Enum to denote shipment direction.
public enum ShipmentType
{
    Inbound,
    Outbound
}
public enum UserRole
{
    Admin,
    Manager,
    Warehouseman,
    Guest
}

// Shop table: holds shop details.
public class Shop
{
    public int ShopId { get; set; }
    public string ShopName { get; set; }
    public string Location { get; set; }

    // Navigation property: One shop can have many shipments.
    public List<Shipment> Shipments { get; set; }

    public static void Validate(string? shopName, string? location)
    {
        if (string.IsNullOrEmpty(shopName) || string.IsNullOrEmpty(location))
        {
            throw new ArgumentNullException(null, "Shop's name and location can't be empty!");
        }
    }
}

// Product table: holds product details.
public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string Unit { get; set; }
    public string Description { get; set; }

    // Navigation: Each product has one stock record.
    public Stock Stock { get; set; }

    // Navigation: One product can appear in many shipment items.
    public List<ShipmentItem> ShipmentItems { get; set; }

    public static void Validate(string? name, string? category, string? unit, string? description)
    {
        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(unit) || string.IsNullOrEmpty(description))
        { 
            throw new ArgumentNullException(null, "Product's name, category, unit, description can't be empty!"); 
        }    
    }
}

// Stock table: maintains current inventory per product.
public class Stock
{
    public int StockId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }

    // Navigation: Reference back to the product.
    public Product Product { get; set; }

    public static void Validate(int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("The stock's quantity can't be smaller than 0!");
        }
    }

    public static void Validate(int quantity, DateTime date)
    {
        if(quantity < 0)
        {
            throw new ArgumentException("The stock's quantity can't be smaller than 0!");
        }
        else if(date > DateTime.Now)
        {
            throw new ArgumentException("The stock's last update date can't be from the future!");
        }
    }
}

// Shipper table: represents external shipping companies.
public class Shipper
{
    public int ShipperId { get; set; }
    public string Name { get; set; }
    public string ContactInfo { get; set; }

    // Navigation: One shipper can be associated with many shipments.
    public List<Shipment> Shipments { get; set; }

    public static void Validate(string? name, string? contactInfo)
    {
        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(contactInfo))
        {
            throw new ArgumentNullException(null, "Shipper's name and contact info can't be empty!");
        }
    }
}

// Shipment table: represents shipments involving the warehouse.
public class Shipment
{
    public int ShipmentId { get; set; }
    public int? ShopId { get; set; }

    // Optional foreign key to a shipper.
    public int? ShipperId { get; set; }

    // Optional foreign key to a user.
    public int? UserId { get; set; }

    // Indicates whether the shipment is inbound or outbound.
    public ShipmentType ShipmentType { get; set; }
    public DateTime? ShippedDate { get; set; }
    public bool IsCompleted { get; set; }

    // Navigation: Each shipment belongs to one shop.
    public Shop? Shop { get; set; }

    // Navigation: Shipment may be associated with an external shipper.
    public Shipper? Shipper { get; set; }

    // Navigation: Shipment may be associated with a user.
    public User? User { get; set; }

    // Navigation: One shipment contains multiple shipment items.
    public List<ShipmentItem> ShipmentItems { get; set; }

    public static void Validate(string? shipmentType)
    {
        if(!Enum.TryParse(shipmentType, out ShipmentType result))
        {
            throw new ArgumentException("Shipment's type was incorrect");
        }
    }
}

// ShipmentItem table: holds details of products in each shipment.
public class ShipmentItem
{
    public int ShipmentItemId { get; set; }
    public int ShipmentId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    // Navigation: Each shipment item belongs to one shipment.
    public Shipment Shipment { get; set; }

    // Navigation: Each shipment item refers to one product.
    public Product Product { get; set; }

    public static void Validate(int quantity)
    {
        if(quantity < 0)
        {
            throw new ArgumentException("The shipment's item quantity can't be smaller than 0!");
        }
    }
}

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public UserRole Role { get; set; } 

    // Navigation property: One user can have many shipments.
    public List<Shipment> Shipments { get; set; } 

    public static void Validate(string? username, string? role)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentNullException(null, "User's username can't be empty");
        }
        else if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_.πÊÍ≥ÒÛúüø•∆ £—”åèØ]+$"))
        {
            throw new ArgumentException("The username can only contain letters, numbers, characters '_' and '.'! ");
        }
        else if(!Enum.TryParse(role, out UserRole userRole))
        {
            throw new ArgumentException("User's role was incorrect");
        }
    }
}
