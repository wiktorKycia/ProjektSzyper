using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

class Dialog
{
    private readonly string Title;
    private readonly string Text;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public Dialog(string text, Action onExit, Action onAccept)
    {
        Title = "Dialog";
        Text = text;
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, onExit.Invoke },
            { ConsoleKey.Enter, ()=> { onAccept.Invoke(); onExit.Invoke(); } }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "cancel" },
            { "<Enter>", "accept" }
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