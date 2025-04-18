using StorageOffice.classes.CLI;
using StorageOffice.classes.Logic;

using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;


using StorageOffice.classes.database;

namespace StorageOffice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            classes.UsersManagement.Modules.User user = new();
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            MenuHandler.Start(user);

            // var db = new StorageDatabase();
            // db.ClearDatabase();
            // db.SeedData(); // Add this line to populate the database

        }
    }
}
