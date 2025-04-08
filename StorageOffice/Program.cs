using StorageOffice.classes.CLI;
using StorageOffice.classes.database;
namespace StorageOffice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var db = new StorageDatabase();
            db.SeedData(); // Add this line to populate the database
        }
    }
}
