using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Services;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

public class EditUser
{
    private readonly string _title;
    private readonly Select _select;
    private readonly Action _onExit;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    internal EditUser(string title, Select select, Action onExit)
    {
        _title = title;
        _select = select;
        _onExit = onExit;
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.UpArrow, select.MoveUp },
            { ConsoleKey.DownArrow, select.MoveDown },
            { ConsoleKey.Enter, select.InvokeOperation },
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "\u2191", "move up" },
            { "\u2193", "move down" },
            { "<Enter>", "select" },
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
                // if (key == ConsoleKey.Escape)
                // {
                //     running = false;
                // }
                // if (key == ConsoleKey.Enter)
                // {
                //     running = false;
                // }
            }
        }
    }

    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = "Select a user to edit:";
        
        if (_select.Options != null)
        {
            foreach (var option in _select.Options)
            {
                content += "\n" + ConsoleOutput.CenteredText(option?.ToString() + "\n" ?? "[ ] No Text" + "\n", true);
            }
        }
        else
        {
            content += "\n" + ConsoleOutput.CenteredText("No users available to edit", true);
        }
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
