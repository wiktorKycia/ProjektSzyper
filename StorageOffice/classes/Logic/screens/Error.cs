using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

class Error
{
    private readonly string _title;
    private readonly string _text;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public Error(string text, Action onExit)
    {
        _title = "Error";
        _text = text;
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
        ConsoleOutput.PrintColorMessage(ConsoleOutput.UIFrame(_title, _text), ConsoleColor.Red);

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}