using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the menu for editing a user's username.
/// This class provides an interactive console-based workflow for changing the username
/// of a specific user.
/// </summary>
/// <remarks>
/// The menu allows the user to input a new username, confirm the change, and update
/// the username in the system.
/// </remarks>
/// <param name="username">
/// The current username of the user whose username is being changed.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user exits the username edit menu.
/// </param>
public class EditUsername
{
    private readonly string _title;
    private readonly string _username;
    private readonly Action _onExit;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    internal EditUsername(string username, Action onExit)
    {
        _title = "Edit Username";
        _username = username;
        _onExit = onExit;
        
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };
        
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "cancel" },
            { "Any other key", "enter new username" }
        };
        
        Run();
    }

    /// <summary>
    /// Executes the main workflow for the username edit menu.
    /// Displays the user interface, handles user input for entering a new username,
    /// and updates the username upon confirmation.
    /// </summary>
    /// <remarks>
    /// This method runs in a loop until the user exits the menu or successfully changes
    /// the username. It ensures proper validation of the new username and handles errors gracefully.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown if the entered username does not meet the required criteria.
    /// </exception>
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
                running = false;
            }
            else
            {
                try
                {
                    string newUsername = ConsoleInput.GetUserString("Enter new username: ");
                    
                    if(GetConfirm(ref running))
                    {
                        int userId = MenuHandler.db?.GetUserIdByUsername(_username) ?? 0;
                        MenuHandler.db?.UpdateUser(userId, newUsername, null);

                        try
                        {
                            PasswordManager.ChangeUsername(_username, newUsername);
                        }
                        catch(Exception e)
                        {
                            running = false;
                            var menu = new Error(
                                text: e.Message,
                                onExit: _onExit.Invoke
                            );
                        }

                        ConsoleOutput.PrintColorMessage($"Username successfully changed to {newUsername}\n", ConsoleColor.Green);
                        Console.WriteLine("Press any key to continue...");
                        ConsoleInput.WaitForAnyKey();
                        running = false;
                        _onExit.Invoke();
                    }
                }
                catch (ArgumentException e)
                {
                    ConsoleOutput.PrintColorMessage(e.Message, ConsoleColor.Red);
                    Console.WriteLine("Press any key to try again...");
                    ConsoleInput.WaitForAnyKey();
                }
            }
        }
    }

    /// <summary>
    /// Prompts the user to confirm the entered username.
    /// Updates the running flag based on the user's confirmation.
    /// </summary>
    /// <param name="running">
    /// A reference to a boolean flag indicating whether the process should continue running.
    /// This flag is set to false if the user confirms the username.
    /// </param>
    /// <returns>
    /// True if the user confirms the username, otherwise false.
    /// </returns>
    private bool GetConfirm(ref bool running)
    {
        Console.WriteLine("Is the username correct? (y/n): ");
        var key = ConsoleInput.GetConsoleKey();
        if (key == ConsoleKey.Y) 
        {
            running = false;
            return true;
        }
        else return false;   
    }

    /// <summary>
    /// Displays the user interface for the username edit menu.
    /// Shows the current username being edited and provides navigation instructions for the user.
    /// </summary>
    /// <remarks>
    /// The method ensures proper formatting of the console output and clears the screen
    /// before displaying the content.
    /// </remarks>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = $"Change username for: {_username}";
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
        
        Console.WriteLine(ConsoleOutput.HorizontalLine('-'));
    }
}
