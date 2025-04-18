using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

public class EditRole
{
    private readonly string _title;
    private readonly string _username;
    private readonly Action _onExit;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    internal EditRole(string username, Action onExit)
    {
        _title = "Edit Role";
        _username = username;
        _onExit = onExit;
        
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };
        
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "cancel" },
            { "Any other key", "select new role" }
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
                running = false;
            }
            else
            {
                try
                {
                    Role newRole = GetRole();
                    
                    if(GetConfirm(newRole))
                    {
                        int userId = MenuHandler.db?.GetUserIdByUsername(_username) ?? 0;
                        PasswordManager.ChangeUserRole(_username, newRole);
                        if (MenuHandler.db != null)
                        {
                            MenuHandler.db.UpdateUser(userId, null, newRole.ToString());
                        }
                        ConsoleOutput.PrintColorMessage($"Role successfully changed to {newRole}", ConsoleColor.Green);
                        Console.WriteLine("Press any key to continue...");
                        ConsoleInput.WaitForAnyKey();
                    }
                    running = false;
                    _onExit.Invoke();
                }
                catch (ArgumentException e)
                {
                    ConsoleOutput.PrintColorMessage(e.Message, ConsoleColor.Red);
                    Console.WriteLine("Press any key to try again...");
                    ConsoleInput.WaitForAnyKey();
                }
            }
        }
    }

    private Role GetRole()
    {
        while(true)
        {
            try
            {
                Console.WriteLine("Available roles:");
                Console.WriteLine("1. Administrator");
                Console.WriteLine("2. Warehouseman");
                Console.WriteLine("3. Logistician");
                Console.WriteLine("4. Warehouse Manager");
                int roleIndex = ConsoleInput.GetUserInt("Enter the role number (1-4): ");
                if (Enum.IsDefined(typeof(Role), roleIndex))
                {
                    return (Role)roleIndex;
                }
                else
                {
                    throw new ArgumentException("Invalid role. Please enter a number between 1 and 4.");
                }
            }
            catch (ArgumentException e)
            {
                ConsoleOutput.PrintColorMessage(e.Message, ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
        }
    }

    private bool GetConfirm(Role newRole)
    {
        string answer = ConsoleInput.GetUserString($"Are you sure you want to change the role to {newRole}? (y/n): ");
        return answer.ToLower() == "y";
    }

    private void Display()
    {
        Console.Clear();
        string content = $"Change role for user: {_username}";
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
        
        Console.WriteLine(ConsoleOutput.HorizontalLine('-'));
    }
}
