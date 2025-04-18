using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

public class EditUsername
{
    private readonly string _title;
    private readonly string _username;
    private readonly Action _onExit;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    internal EditUsername(string username, Action onExit)
    {
        _title = "Edit Username";
        _username = username;
        _onExit = onExit;
        
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };
        
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "cancel" },
            { "Any other key", "enter new username" }
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
                    string newUsername = ConsoleInput.GetUserString("Enter new username: ");
                    
                    if(GetConfirm())
                    {
                        int userId = MenuHandler.db?.GetUserIdByUsername(_username) ?? 0;
                        PasswordManager.ChangeUsername(_username, newUsername);
                        if (MenuHandler.db != null)
                        {
                            MenuHandler.db.UpdateUser(userId, newUsername, null);
                        }
                        ConsoleOutput.PrintColorMessage($"Username successfully changed to {newUsername}", ConsoleColor.Green);
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

    private bool GetConfirm()
    {
        string answer = ConsoleInput.GetUserString("Are you sure you want to change the username? (y/n): ");
        return answer.ToLower() == "y";
    }

    private void Display()
    {
        Console.Clear();
        string content = $"Change username for: {_username}";
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
        
        Console.WriteLine(ConsoleOutput.HorizontalLine('-'));
    }
}
