using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

public class DeleteUser
{
    private readonly string _title;
    private readonly string _heading;
    private readonly Select _select;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public DeleteUser(CheckBoxSelect select, Action onExit)
    {
        _title = "Delete User";
        _heading = "Choose users to delete";
        _select = select;
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.UpArrow, select.MoveUp },
            { ConsoleKey.DownArrow, select.MoveDown },
            { ConsoleKey.Enter, select.SelectOption },
            { ConsoleKey.Delete, () => Confirm(onExit) },
            { ConsoleKey.Escape, onExit.Invoke }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "\u2191", "move up" },
            { "\u2193", "move down" },
            { "<Enter>", "select" },
            { "<Del>", "delete" },
            { "<Esc>", "back" }
        };
        Run();
    }
    private void Confirm(Action exit)
    {
        Console.Clear();
        Console.WriteLine(ConsoleOutput.Header("Confirm Deletion", '-') + "\n");
        Console.WriteLine("Are you sure you want to delete the selected users? (y/n)");

        var key = ConsoleInput.GetConsoleKey();
        if (key == ConsoleKey.Y)
        {
            _select.InvokeOperation();
            ConsoleOutput.PrintColorMessage("Users successfully deleted!\n", ConsoleColor.Green);
            Console.WriteLine("Press any key to continue...");
            ConsoleInput.WaitForAnyKey();
            exit.Invoke();
        }
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

    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");

        Console.WriteLine(ConsoleOutput.Header(_title, '=') + "\n");
        Console.WriteLine(_heading.RightMargin() + "\n");

        
        if (_select.Options != null)
        {
            foreach (CheckBoxOption option in _select.Options)
            {
                if (option.IsHighlighted)
                {
                    ConsoleOutput.PrintColorMessage(ConsoleOutput.CenteredText(option?.ToString() + "\n" ?? "[ ] No Text" + "\n", true), ConsoleColor.Blue);
                }
                else
                {
                    Console.WriteLine(ConsoleOutput.CenteredText(option?.ToString() + "\n" ?? "[ ] No Text" + "\n", true));
                }
            }
        }
        else
        {
            Console.WriteLine(ConsoleOutput.CenteredText("No options available", true));
        }
        
        Console.WriteLine(ConsoleOutput.HorizontalLine('='));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}