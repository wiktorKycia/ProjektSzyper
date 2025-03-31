using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

public class AppContext : DbContext
{

}


public class Product
{
    public int Id {get;set;}
    public string Name {get;set;}
    public int Quantity {get;set;}
    public string Unit {get;set;}
}

public class Transaction
{
    public int Id {get;set;}
    public string SubjectName {get;set;}
    public List<Product> Products {get;set;}
    public DateTime Date {get;set;}
    public int Quantity {get;set;}
    public string Unit {get;set;}
    public string Type {get;set;}
}