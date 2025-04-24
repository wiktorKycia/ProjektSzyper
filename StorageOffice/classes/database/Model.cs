using Bogus.Bson;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace StorageOffice.classes.database;

/// <summary>
/// StorageContext class: represents the database context for the storage office application.
/// It inherits from DbContext and provides access to the database tables.
/// </summary>
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

    /// <summary>
    /// Constructor for the StorageContext class.
    /// Initializes the database path based on the solution's directory or a custom path.
    /// </summary>
    /// <param name="customDbPath"></param>
    public StorageContext(string? customDbPath = null)
    {
        var solutionDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
        var appPath = Path.Combine(solutionDirectory, "Data");

        // Create the Data folder in the solution's base directory if it doesn't exist yet
        Directory.CreateDirectory(appPath);

        this.DbPath = customDbPath ?? Path.Combine(appPath, "StorageOffice.db");
    }

    /// <summary>
    /// Configures the database context to use SQLite.
    /// </summary>
    /// <param name="options"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}", opt =>
        {
            opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });
    }

    /// <summary>
    /// Configures the model relationships and constraints.
    /// </summary>
    /// <param name="modelBuilder"></param>
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


/// <summary>
/// Enum to denote shipment direction.
/// </summary>
public enum ShipmentType
{
    Inbound,
    Outbound
}

/// <summary>
/// Enum to denote user roles.
/// </summary>
public enum UserRole
{
    Administrator = 1,
    Warehouseman,
    Logistician,
    WarehouseManager
}

/// <summary>
/// Shop table: holds shop details.
/// </summary>
public class Shop
{
    public int ShopId { get; set; }
    public string ShopName { get; set; }
    public string Location { get; set; }

    // Navigation property: One shop can have many shipments.
    public List<Shipment> Shipments { get; set; }

    /// <summary>
    /// Validates the correctness of the shop's data. Both shop's name and shop's location can't be empty or null.
    /// </summary>
    /// <param name="shopName">String representing shop's name that has to be checked.</param>
    /// <param name="location">String representing shop's location that has to be checked.</param>
    /// <exception cref="ArgumentException">This exception is thrown when shop's name or location is equal to null or is empty.</exception>
    public static void Validate(string? shopName, string? location)
    {
        if (string.IsNullOrEmpty(shopName) || string.IsNullOrEmpty(location))
        {
            throw new ArgumentException(null, "Shop's name and location can't be empty!");
        }
    }
}

/// <summary>
/// Product table: holds product details.
/// </summary>
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

    /// <summary>
    /// Validates the correctness of the product's data. Products's name, category, unit and description can't be empty or null.
    /// </summary>
    /// <param name="name">String representing products's name that has to be checked.</param>
    /// <param name="category">String representing products's category that has to be checked.</param>
    /// <param name="unit">String representing products's unit that has to be checked.</param>
    /// <param name="description">String representing products's description that has to be checked.</param>
    /// <exception cref="ArgumentException">This exception is thrown when product's name, category, unit or description is equal to null or is empty.</exception>
    public static void Validate(string? name, string? category, string? unit, string? description)
    {
        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(unit) || string.IsNullOrEmpty(description))
        { 
            throw new ArgumentException(null, "Product's name, category, unit, description can't be empty!"); 
        }    
    }
}

/// <summary>
/// Stock table: maintains current inventory per product.
/// </summary>
public class Stock
{
    public int StockId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }

    // Navigation: Reference back to the product.
    public Product Product { get; set; }

    /// <summary>
    /// Validates the correctness of the stock's data. Stock's quantity can't be smaller than 0.
    /// </summary>
    /// <param name="quantity">Integer representing stock's quantity that has to be checked.</param>
    /// <exception cref="ArgumentException">This exception is thrown when stock's quantity is smaller than 0.</exception>
    public static void Validate(int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("The stock's quantity can't be smaller than 0!");
        }
    }

    /// <summary>
    /// Validates the correctness of the stock's data. Stock's quantity can't be smaller than 0 and stock's update date can't be from the future.
    /// </summary>
    /// <param name="quantity">Integer representing stock's quantity that has to be checked.</param>
    /// <param name="date">DateTime object representing stock's update date that has to be checked.</param>
    /// <exception cref="ArgumentException">This exception is thrown when stock's quantity is smaller than 0 or stock's update date is from the future.</exception>
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

/// <summary>
/// Shipper table: represents external shipping companies.
/// </summary>
public class Shipper
{
    public int ShipperId { get; set; }
    public string Name { get; set; }
    public string ContactInfo { get; set; }

    // Navigation: One shipper can be associated with many shipments.
    public List<Shipment> Shipments { get; set; }

    /// <summary>
    /// Validates the correctness of the shippers's data. Shippers's name and contact info can't be null or empty.
    /// </summary>
    /// <param name="name">String representing shippers's name that has to be checked.</param>
    /// <param name="contactInfo">String representing shipper's contact info that has to be checked.</param>
    /// <exception cref="ArgumentException">This exception is thrown when shipper's name or contact info is equal to null or is empty.</exception>
    public static void Validate(string? name, string? contactInfo)
    {
        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(contactInfo))
        {
            throw new ArgumentException(null, "Shipper's name and contact info can't be empty!");
        }
    }
}

/// <summary>
/// Shipment table: represents shipments involving the warehouse.
/// </summary>
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

    /// <summary>
    /// Validates the correctness of the shipment's data. Shipment's type must be Inbound or Outbound.
    /// </summary>
    /// <param name="shipmentType">String representing shipment's type that has to be checked.</param>
    /// <exception cref="ArgumentException">This exception is thrown when shipment's type is not equal to Inbound or Outbound.</exception>
    public static void Validate(string? shipmentType)
    {
        if(!Enum.TryParse(shipmentType, out ShipmentType result))
        {
            throw new ArgumentException("Shipment's type was incorrect");
        }
    }
}

/// <summary>
/// ShipmentItem table: holds details of products in each shipment.
/// </summary>
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

    /// <summary>
    /// Validates the correctness of the shipment item's data. Item's quantity can't be smaller than 0.
    /// </summary>
    /// <param name="quantity">Integer representing item's quantity that has to be checked.</param>
    /// <exception cref="ArgumentException">This exception is thrown when item's quantity is smaller than 0.</exception>
    public static void Validate(int quantity)
    {
        if(quantity < 0)
        {
            throw new ArgumentException("The shipment's item quantity can't be smaller than 0!");
        }
    }
}

/// <summary>
/// User table: represents users of the system.
/// </summary>
public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public UserRole Role { get; set; } 

    // Navigation property: One user can have many shipments.
    public List<Shipment> Shipments { get; set; }

    /// <summary>
    /// Validates the correctness of the user's data. User's name and role must not be empty or null and must fit into correct values or regex.
    /// </summary>
    /// <param name="username">String representing user's name that has to be checked.</param>
    /// <param name="role">String representing user's role that has to be checked.</param>
    /// <exception cref="ArgumentException">This exception is thrown when user's name is empty, null or doesn't fit into correct regex or user's role is not a correct role.</exception>
    public static void Validate(string? username, string? role)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException(null, "User's username can't be empty");
        }
        else if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_.ąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
        {
            throw new ArgumentException("The username can only contain letters, numbers, characters '_' and '.'! ");
        }
        else if(!Enum.TryParse(role, out UserRole userRole))
        {
            throw new ArgumentException("User's role was incorrect");
        }
    }
}
