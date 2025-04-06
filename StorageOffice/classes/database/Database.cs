using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageOffice.classes.database
{
    class StorageDatabase
    {
        private StorageContext _context = new();
        public string Path { get { return _context.DbPath; } }

        public StorageDatabase()
        {
            _context.Database.SetCommandTimeout(5);
            _context.Database.Migrate();
        }
        // tu piszemy metody do dodawania, usuwania i edytowania danych w bazie

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(int productId, Product updatedProduct)
        {
            var existingProduct = _context.Products.Find(productId);
            if (existingProduct != null)
            {
                existingProduct.Name = updatedProduct.Name;
                existingProduct.Quantity = updatedProduct.Quantity;
                existingProduct.Unit = updatedProduct.Unit;
                _context.SaveChanges();
            }
        }

        public void DeleteProduct(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public List<Product> GetAllProducts() => [.. _context.Products];

        public Product? GetProductById(int productId) => _context.Products.Find(productId);
    }
}
