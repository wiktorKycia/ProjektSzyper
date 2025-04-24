using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the logs screen for displaying system logs.
/// This class provides a simple console-based interface to display logs
/// and allows the user to exit the screen.
/// </summary>
/// <remarks>
/// The logs screen is designed to be used in a console application and ensures
/// proper formatting of the log content.
/// </remarks>
/// <param name="text">
/// The log content to be displayed.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user exits the logs screen.
/// </param>
class Logs
{
    private readonly string _title;
    private readonly string _text;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public Logs(string text, Action onExit)
    {
        _title = "Logs";
        _text = text;
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, onExit.Invoke }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "back" }
        };
        Run();
    }

    /// <summary>
    /// Executes the main workflow for the logs screen.
    /// Displays the log content and handles user input for exiting the screen.
    /// </summary>
    /// <remarks>
    /// This method runs in a loop until the user exits the screen by pressing the appropriate key.
    /// </remarks>
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

    /// <summary>
    /// Displays the user interface for the logs screen.
    /// Shows the log content and provides navigation instructions for the user.
    /// </summary>
    /// <remarks>
    /// The method ensures proper formatting of the console output and clears the screen
    /// before displaying the content.
    /// </remarks>
    public void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        Console.WriteLine(ConsoleOutput.UIFrame(_title, _text));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}