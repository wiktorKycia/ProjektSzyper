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
    private readonly Action _nextMenu;
    private readonly User _user;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    internal FirstUser(string title, string heading, Action nextMenu, User user)
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
        _nextMenu = nextMenu;
        _user = user;
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
                GetUsername(_user);
                string password = GetPassword();

                if(GetConfirm(ref running))
                {
                    PasswordManager.SaveNewUser(_user.Username, password, Role.Administrator);                    
                    MenuHandler.db?.AddUser(_user.Username, "Administrator");
                    
                    LogManager.AddNewLog($"Info: login of user {_user.Username} - successful");
                    ConsoleOutput.PrintColorMessage("User successfully created\n", ConsoleColor.Green);
                    Console.WriteLine("Press any key to continue...");
                    ConsoleInput.WaitForAnyKey();
                    _nextMenu.Invoke();
                }
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
    private bool GetConfirm(ref bool running)
    {
        string answer = ConsoleInput.GetUserString("Is the information correct? (y/n): ");
        if (answer.ToLower() == "y")
        {
            running = false;
            return true;
        }
        return false;
    }

    private void Display()
    {
        Console.Clear();
        string content = _heading;
        // string content = CenteredText(_heading, true) + "\n" + "No user has been created on the system, so the details currently provided will be used to create the first administrator. Enter the details for him/her.\n";
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content.RightMargin()));
        
        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }

        Console.WriteLine(ConsoleOutput.HorizontalLine('-'));

    }
}