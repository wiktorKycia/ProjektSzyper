using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents a dialog screen for user interaction.
/// This class provides a simple console-based interface to display a message
/// and allows the user to either accept or cancel the dialog.
/// </summary>
/// <remarks>
/// The dialog is designed to be used in a console application and supports
/// customizable actions for both acceptance and cancellation.
/// </remarks>
/// <param name="text">
/// The text content to be displayed in the dialog.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user cancels or exits the dialog.
/// </param>
/// <param name="onAccept">
/// An action to be invoked when the user accepts the dialog.
/// </param>
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

    /// <summary>
    /// Executes the main workflow for the dialog screen.
    /// Displays the dialog content and handles user input for accepting or canceling the dialog.
    /// </summary>
    /// <remarks>
    /// This method runs in a loop until the user provides input to either accept or cancel the dialog.
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
    /// Displays the user interface for the dialog screen.
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