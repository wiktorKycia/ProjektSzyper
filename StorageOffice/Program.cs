using StorageOffice.classes.CLI;
using StorageOffice.classes.Logic;

using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Models;
using StorageOffice.classes.UsersManagement.Services;


using StorageOffice.classes.database;

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
            classes.UsersManagement.Models.User user = new();

            PasswordManager.currentUser = user;

            try
            {
                // Start the application
                MenuHandler.Start(user);
            }
            catch(FormatException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Probably there is a redundant empty line in users.txt file or logs.txt file.");
                Console.WriteLine("Please check the file and try again.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Please check if the users.txt and logs.txt files exist and try again.");
                Console.WriteLine("Press any key to exit.");
            }
        }
    }
}
