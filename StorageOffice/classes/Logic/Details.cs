using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

class DetailsMenu
{
    public readonly string Title;
    public readonly string Text;
    public Dictionary<ConsoleKey, KeyboardAction> KeyboardActions { get; set; }
    public Dictionary<string, string> DisplayKeyboardActions { get; set; }
    public DetailsMenu(string text)
    {
        Title = "Details";
        Text = text;
        KeyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, () => Environment.Exit(0) }
        };
        DisplayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "exit" }
        };
        Display();
        var key = ConsoleInput.GetConsoleKey();
        if (KeyboardActions.ContainsKey(key))
        {
            KeyboardActions[key]?.Invoke();
        }
    }
    public void Display()
    {
        Console.Clear();
        Console.WriteLine(ConsoleOutput.UIFrame(Title, Text));

        foreach (var action in DisplayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}