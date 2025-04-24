using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Services;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the menu for editing a specific user.
/// This class provides an interactive console-based workflow for selecting and performing
/// edit operations on a specific user.
/// </summary>
/// <remarks>
/// The menu allows navigation through options and invokes the appropriate operation
/// based on the user's selection.
/// </remarks>
/// <param name="username">
/// The username of the user being edited.
/// </param>
/// <param name="select">
/// A <see cref="Select"/> instance containing the list of options available for editing the user.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user exits the edit menu.
/// </param>
public class EditConcreteUser
{
    private readonly string _title;
    private readonly string _username;
    private readonly Action _onExit;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;
    private readonly Select _select;

    internal EditConcreteUser(string username, Select select, Action onExit)
    {
        _title = "Edit User";
        _username = username;
        _onExit = onExit;

        _select = select;

        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>()
        {
            { ConsoleKey.UpArrow, _select.MoveUp },
            { ConsoleKey.DownArrow, _select.MoveDown },
            { ConsoleKey.Enter, _select.InvokeOperation },
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };

        _displayKeyboardActions = new Dictionary<string, string>()
        {
            { "\u2191", "move up" },
            { "\u2193", "move down" },
            { "<Enter>", "select" },
            { "<Esc>", "back" }
        };

        Run();
    }

    /// <summary>
    /// Executes the main workflow for the user edit menu.
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
        while (running)
        {
            Display();
            var key = ConsoleInput.GetConsoleKey();
            if (_keyboardActions.ContainsKey(key))
            {
                if (key == ConsoleKey.Escape)
                {
                    running = false;
                    _onExit.Invoke();
                }
                else
                {
                    _keyboardActions[key]();
                }
            }
        }
    }

    /// <summary>
    /// Displays the user interface for the user edit menu.
    /// Shows the username being edited, the list of available options, and navigation instructions
    /// for the user.
    /// </summary>
    /// <remarks>
    /// The method dynamically builds the content to display the menu options and ensures
    /// proper formatting for the console output.
    /// </remarks>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = $"Editing user: {_username}\n\nSelect an option:";

        foreach (var option in _select.Options)
        {
            content += "\n" + ConsoleOutput.CenteredText(option?.ToString() + "\n" ?? "[ ] No Text" + "\n", true);
        }

        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
