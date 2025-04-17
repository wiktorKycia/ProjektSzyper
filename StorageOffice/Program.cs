using StorageOffice.classes.CLI;
using StorageOffice.classes.Logic;

using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // Console.OutputEncoding = System.Text.Encoding.Unicode;
            // MenuHandler.MainMenu();

            bool loggedIn = true;
            RBAC rbacSystem = new RBAC();

            while (true)
            {
                Console.Clear();
                bool isAnyUserCreated = PasswordManager.CheckFile();
                if (!isAnyUserCreated)
                {
                    Console.WriteLine("No user has been created on the system, so the details currently provided will be used to create the first administrator. Enter the details for him/her.\n");
                }
                Console.WriteLine("Welcome to the logistics warehouse management system!");

                User user = new User();
                bool isUsernameCorrect = false;
                string username = "";
                while (!isUsernameCorrect)
                {
                    try
                    {
                        if (isAnyUserCreated)
                        {
                            Console.Write("Enter your username: ");
                        }
                        else
                        {
                            Console.Write("Enter the username of the first administrator: ");
                        }
                        username = Console.ReadLine();
                        user.Username = username;
                        isUsernameCorrect = true;
                    }
                    catch (ArgumentNullException e)
                    {
                        Console.Write(e.Message);
                    }
                    catch (ArgumentException e)
                    {
                        Console.Write(e.Message);
                    }
                }

                bool isPasswordCorrect = false;
                string password = "";
                while (!isPasswordCorrect)
                {
                    if (isAnyUserCreated)
                    {
                        Console.Write("Enter your password: ");
                    }
                    else
                    {
                        Console.Write("Enter the password of the first administrator: ");
                    }
                    
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

                if (!isAnyUserCreated)
                {
                    PasswordManager.SaveNewUser(username, password, Role.Administrator);
                    LogManager.AddNewLog($"Info: login of user {username} - successful");
                }
                else
                {
                    Role? role = PasswordManager.VerifyPasswordAndGetRole(username, password);
                    if (role == null)
                    {
                        Console.WriteLine("Username or password is incorrect. Press any key and try again");
                        Console.ReadKey();
                        continue;
                    }
                    else
                    {
                        user.Role = (Role)role;
                    }
                }

                
                loggedIn = true;
                Console.WriteLine("LoggedIn");
                Console.ReadKey();
            }
        }
    }
}
