using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents an error screen for displaying error messages.
/// This class provides a simple console-based interface to display an error message
/// and allows the user to exit the screen.
/// </summary>
/// <remarks>
/// The error screen is designed to be used in a console application and ensures
/// proper formatting of the error message.
/// </remarks>
/// <param name="text">
/// The error message to be displayed.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user exits the error screen.
/// </param>
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

    /// <summary>
    /// Executes the main workflow for the error screen.
    /// Displays the error message and handles user input for exiting the screen.
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
    /// Displays the user interface for the error screen.
    /// Shows the error message and provides navigation instructions for the user.
    /// </summary>
    /// <remarks>
    /// The method ensures proper formatting of the console output and clears the screen
    /// before displaying the content.
    /// </remarks>
    public void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        ConsoleOutput.PrintColorMessage(ConsoleOutput.UIFrame(_title, _text), ConsoleColor.Red);

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}