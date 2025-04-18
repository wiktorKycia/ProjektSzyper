using StorageOffice.classes.CLI;
using StorageOffice.classes.Logic;

using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;


using StorageOffice.classes.database;
using StorageOffice.classes.TestsServices.Modules;
using StorageOffice.classes.Tests.Modules;

namespace StorageOffice
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Loading...");

            Console.OutputEncoding = System.Text.Encoding.Unicode;

            // Create database and seed data
            var db = new StorageDatabase();

            // db.ClearDatabase();
            // db.SeedData();
            MenuHandler.db = db;

            // Create an object to store the info about logged-in user
            classes.UsersManagement.Modules.User user = new();

            // Start the application
            MenuHandler.Start(user);
        }
    }
}
