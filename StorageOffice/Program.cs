using StorageOffice.classes.CLI;
using StorageOffice.classes.database;
namespace StorageOffice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var db = new StorageDatabase();
            db.ClearDatabase();
            db.SeedData(); // Add this line to populate the database
            db.AddProduct("Janko", "człowiek", "pkg", "An 17 years old man");
        }
    }
}
