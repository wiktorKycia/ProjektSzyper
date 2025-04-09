using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;
namespace StorageOffice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                bool isAnyUserCreated = PasswordManager.CheckFile();
                if (!isAnyUserCreated)
                {
                    Console.WriteLine("No users created in the system. Start creating a new user - administrator. Please enter details for him/her.");
                }
                Console.Clear();
                Console.WriteLine("Welcome to the logistics warehouse management system!");

                User user = new User();
                bool isUsernameCorrect = false;
                string username = "";
                while (!isUsernameCorrect)
                {
                    try
                    {
                        Console.Write("Enter your user name: ");
                        username = Console.ReadLine();
                        user.Username = username;
                        isUsernameCorrect = true;
                    }
                    catch (ArgumentNullException e)
                    {
                        Console.Write(e.Message);
                    }
                }

                bool isPasswordCorrect = false;
                string password = "";
                while (!isPasswordCorrect)
                {
                    Console.Write("Enter your password: ");
                    password = Console.ReadLine();
                    if (string.IsNullOrEmpty(password))
                    {
                        Console.Write("The password must not be empty! ");
                    }
                    else
                    {
                        isPasswordCorrect = true;
                    }
                }

                if (!PasswordManager.VerifyPassword(username, password))
                {
                    
                }
            }
        }
    }
}
