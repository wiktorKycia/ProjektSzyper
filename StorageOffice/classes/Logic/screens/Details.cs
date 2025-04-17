using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

class Details
{
    private readonly string Title;
    private readonly string Text;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public Details(string text, Action onExit)
    {
        Title = "Details";
        Text = text;
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
        Console.WriteLine(ConsoleOutput.UIFrame(Title, Text));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}