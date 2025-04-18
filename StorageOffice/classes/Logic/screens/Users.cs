using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;
namespace StorageOffice.classes.Logic;

class Users
{
    private readonly string _title;
    private readonly List<User> _users;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public Users(List<User> users, Action onExit)
    {
        _title = "Users";
        _users = users;
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, onExit.Invoke }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "back" }
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
        }
    }

    public void Display()
    {
        Console.Clear();
        string text = string.Empty;

        if (_users != null)
        {
            foreach (var user in _users)
            {
                text += "\n" + ConsoleOutput.CenteredText(user?.ToString() ?? "", true);
            }
        }
        else
        {
            text = "No users found.";
        }

        Console.WriteLine(ConsoleOutput.UIFrame(_title, text));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}