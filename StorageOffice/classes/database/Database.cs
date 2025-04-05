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
    }
}
