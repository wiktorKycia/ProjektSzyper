using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the logic for deleting users from the system.
/// This class provides an interactive console-based workflow for selecting users
/// and confirming their deletion.
/// </summary>
/// <remarks>
/// The menu allows users to navigate through a list of users, select multiple users
/// for deletion, and confirm the deletion process.
/// </remarks>
/// <param name="select">
/// A <see cref="CheckBoxSelect"/> instance containing the list of users to be displayed
/// and selected for deletion.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user exits the deletion menu.
/// </param>
public class DeleteUser
{
    private readonly string _title;
    private readonly string _heading;
    private readonly Select _select;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public DeleteUser(CheckBoxSelect select, Action onExit)
    {
        _title = "Delete User";
        _heading = "Choose users to delete";
        _select = select;
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.UpArrow, select.MoveUp },
            { ConsoleKey.DownArrow, select.MoveDown },
            { ConsoleKey.Enter, select.SelectOption },
            { ConsoleKey.Delete, () => Confirm(onExit) },
            { ConsoleKey.Escape, onExit.Invoke }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "\u2191", "move up" },
            { "\u2193", "move down" },
            { "<Enter>", "select" },
            { "<Del>", "delete" },
            { "<Esc>", "back" }
        };
        Run();
    }
    /// <summary>
    /// Confirms the deletion of the selected users.
    /// Displays a confirmation prompt to the user and deletes the selected users
    /// if the user confirms the action.
    /// </summary>
    /// <param name="exit">
    /// An action to be invoked after the deletion process is completed or canceled.
    /// </param>
    /// <remarks>
    /// If the user confirms the deletion, the selected users are deleted, and a success
    /// message is displayed. If the user cancels, the action is aborted.
    /// </remarks>
    private void Confirm(Action exit)
    {
        Console.Clear();
        Console.WriteLine(ConsoleOutput.Header("Confirm Deletion", '-') + "\n");
        Console.WriteLine("Are you sure you want to delete the selected users? (y/n)");

        var key = ConsoleInput.GetConsoleKey();
        if (key == ConsoleKey.Y)
        {
            _select.InvokeOperation();
            ConsoleOutput.PrintColorMessage("Users successfully deleted!\n", ConsoleColor.Green);
            Console.WriteLine("Press any key to continue...");
            ConsoleInput.WaitForAnyKey();
            exit.Invoke();
        }
    }

    /// <summary>
    /// Executes the main workflow for the user deletion menu.
    /// Displays the user interface, handles user input for navigation and selection,
    /// and processes the deletion of selected users.
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
    /// Displays the user interface for the user deletion menu.
    /// Shows the list of users available for deletion and provides navigation instructions
    /// for the user.
    /// </summary>
    /// <remarks>
    /// The method dynamically builds the content to display the list of users and ensures
    /// proper formatting for the console output.
    /// </remarks>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");

        Console.WriteLine(ConsoleOutput.Header(_title, '=') + "\n");
        Console.WriteLine(_heading.RightMargin() + "\n");

        
        if (_select.Options != null)
        {
            foreach (CheckBoxOption option in _select.Options)
            {
                if (option.IsHighlighted)
                {
                    ConsoleOutput.PrintColorMessage(ConsoleOutput.CenteredText(option?.ToString() + "\n" ?? "[ ] No Text" + "\n", true), ConsoleColor.Blue);
                }
                else
                {
                    Console.WriteLine(ConsoleOutput.CenteredText(option?.ToString() + "\n" ?? "[ ] No Text" + "\n", true));
                }
            }
        }
        else
        {
            Console.WriteLine(ConsoleOutput.CenteredText("No options available", true));
        }
        
        Console.WriteLine(ConsoleOutput.HorizontalLine('='));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}