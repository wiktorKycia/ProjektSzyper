using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents a screen for displaying detailed information.
/// This class provides a simple console-based interface to display a title and text content,
/// with an option to exit the screen.
/// </summary>
/// <remarks>
/// The class is designed to be used in a console application and allows users to view
/// detailed information with minimal interaction.
/// </remarks>
/// <param name="text">
/// The text content to be displayed on the details screen.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user exits the details screen.
/// </param>
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

    /// <summary>
    /// Executes the main workflow for the details screen.
    /// Displays the content and handles user input for exiting the screen.
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
    /// Displays the user interface for the details screen.
    /// Shows the title and text content, along with navigation instructions for the user.
    /// </summary>
    /// <remarks>
    /// The method ensures proper formatting of the console output and clears the screen
    /// before displaying the content.
    /// </remarks>
    public void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        Console.WriteLine(ConsoleOutput.UIFrame(Title, Text));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}