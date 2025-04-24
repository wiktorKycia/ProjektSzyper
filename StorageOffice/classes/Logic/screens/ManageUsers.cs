using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;


/// <summary>
/// Represents the menu for managing users.
/// This class provides an interactive console-based workflow for viewing and managing
/// user-related operations.
/// </summary>
/// <remarks>
/// The menu allows navigation through a list of user options and invokes the appropriate
/// operation based on the user's selection.
/// </remarks>
/// <param name="title">
/// The title of the menu.
/// </param>
/// <param name="heading">
/// The heading text to be displayed in the menu.
/// </param>
/// <param name="select">
/// A <see cref="Select"/> instance containing the list of user options available in the menu.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user exits the menu.
/// </param>
public class ManageUsers
{
    private readonly string _title;
    private readonly string _heading;
    private readonly Select _select;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public ManageUsers(string title, string heading, Select select, Action onExit)
    {
        _title = title;
        _heading = heading;
        _select = select;
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.UpArrow, select.MoveUp },
            { ConsoleKey.DownArrow, select.MoveDown },
            { ConsoleKey.Enter, select.InvokeOperation },
            { ConsoleKey.Escape, onExit.Invoke }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "\u2191", "move up" },
            { "\u2193", "move down" },
            { "<Enter>", "select" },
            { "<Esc>", "back" }
        };
        Run();
    }

    /// <summary>
    /// Executes the main workflow for the user management menu.
    /// Displays the user interface, handles user input for navigation and selection,
    /// and invokes the appropriate operation based on the selected option.
    /// </summary>
    /// <remarks>
    /// This method runs in a loop until the user exits the menu. It ensures proper handling
    /// of keyboard actions and updates the display accordingly.
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
    /// Displays the user interface for the user management menu.
    /// Shows the list of user options and provides navigation instructions for the user.
    /// </summary>
    /// <remarks>
    /// The method dynamically builds the content to display the menu options and ensures
    /// proper formatting for the console output.
    /// </remarks>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = _heading;
        
        if (_select.Options != null)
        {
            foreach (var option in _select.Options)
            {
                content += "\n" + ConsoleOutput.CenteredText(option?.ToString() + "\n" ?? "[ ] No Text" + "\n", true);
            }
        }
        else
        {
            content += "\n" + ConsoleOutput.CenteredText("No options available", true);
        }
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}