using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

public class Menu
{
    public readonly string Title;
    public readonly string Heading;
    public Select Select { get; set; }
    public Dictionary<ConsoleKey, KeyboardAction> KeyboardActions { get; set; }
    public Dictionary<string, string> DisplayKeyboardActions { get; set; }
    public Menu(string title, string heading, Select select)
    {
        Title = title;
        Heading = heading;
        Select = select;
        KeyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.UpArrow, select.MoveUp },
            { ConsoleKey.DownArrow, select.MoveDown },
            { ConsoleKey.Enter, select.SelectOption },
            { ConsoleKey.Escape, () => Environment.Exit(0) }
        };
        DisplayKeyboardActions = new Dictionary<string, string>(){
            { "\u2191", "move up" },
            { "\u2193", "move down" },
            { "<Enter>", "select" },
            { "<Esc>", "exit" }
        };
        Display();

    }
    public void Run()
    {
        bool running = true;
        while(running)
        {
            Display();
            var key = ConsoleInput.GetConsoleKey();
            if (KeyboardActions.ContainsKey(key))
            {
                KeyboardActions[key]();
            }
        }
    }
    public void Display()
    {
        Console.Clear();
        string content = Heading;
        
        if (Select.Options != null)
        {
            foreach (var option in Select.Options)
            {
                if (option != null)
                {
                    content += "\n" + ConsoleOutput.CenteredText(option.ToString() ?? "[ ] No Text", true);
                }
                else
                {
                    content += "\n" + ConsoleOutput.CenteredText("[ ] Null Option", true);
                }
            }
        }
        else
        {
            content += "\n" + ConsoleOutput.CenteredText("No options available", true);
        }
        
        Console.WriteLine(ConsoleOutput.UIFrame(Title, content));

        foreach (var action in DisplayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}