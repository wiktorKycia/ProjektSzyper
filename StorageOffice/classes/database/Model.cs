using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

public class AppContext : DbContext
{
    public DbSet<Product> Products {get;set;}
    public DbSet<Transaction> Transactions {get;set;}

    public string DbPath {get;}

    public AppContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "blogging.db");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("FileName=StorageOffice.db", opt =>
        {
            opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });
    }
}


public class Product
{
    public int Id {get;set;}
    public string Name {get;set;}
    public int Quantity {get;set;}
    public string Unit {get;set;}

    public List<Transaction> Transactions {get;set;}
}

public class Transaction
{
    public int Id {get;set;}
    public string SubjectName {get;set;}
    public int ProuctId {get;set;}
    public Product Product {get;set;}
    public DateTime Date {get;set;}
    public int Quantity {get;set;}
    public string Unit {get;set;}
    public string Type {get;set;}
}