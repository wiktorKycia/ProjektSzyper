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
