using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

public class FirstUser
{
    private readonly string _title;
    private readonly string _heading;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public FirstUser(string title, string heading)
    {
        _title = title;
        _heading = heading;
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, () => Environment.Exit(0) }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "exit" },
            { "Any other key", "enter username and password" }
        };
        Run();
    }

    private void Run()
    {
        bool running = true;
        while(running)
        {
            Display();
            var key = ConsoleInput.GetConsoleKey();
            if (_keyboardActions.ContainsKey(key))
            {
                _keyboardActions[key]();
            }
            else
            {
                User user = new User();
                GetUsername(user);
                string password = GetPassword();

                PasswordManager.SaveNewUser(user.Username, password, Role.Administrator);
                LogManager.AddNewLog($"Info: login of user {user.Username} - successful");
            }
        }
    }
    private void GetUsername(User user)
    {
        bool isCorrect = false;
        while(!isCorrect)
        {
            try
            {
                user.Username = ConsoleInput.GetUserString("Enter the username of the first administrator: ");
                isCorrect = true;
            }
            catch (ArgumentNullException e)
            {
                ConsoleOutput.PrintColorMessage(e.Message + "\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
            catch (ArgumentException e)
            {
                ConsoleOutput.PrintColorMessage(e.Message + "\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
            catch (Exception e)
            {
                ConsoleOutput.PrintColorMessage($"An unexpected error occurred: {e.Message}\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
        }
    }
    private string GetPassword()
    {
        while(true)
        {
            try
            {
                string password = ConsoleInput.GetUserString("Enter the password of the first administrator: ");
                return password;
            }
            catch (ArgumentNullException e)
            {
                ConsoleOutput.PrintColorMessage(e.Message + "\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
            catch (ArgumentException e)
            {
                ConsoleOutput.PrintColorMessage(e.Message + "\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
            catch (Exception e)
            {
                ConsoleOutput.PrintColorMessage($"An unexpected error occurred: {e.Message}\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
        }
    }

    private void Display()
    {
        Console.Clear();
        string content = _heading;
        // string content = CenteredText(_heading, true) + "\n" + "No user has been created on the system, so the details currently provided will be used to create the first administrator. Enter the details for him/her.\n";
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }

        ConsoleOutput.HorizontalLine('-');

    }
}