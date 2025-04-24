using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the menu for editing a user's role.
/// This class provides an interactive console-based workflow for changing the role
/// of a specific user.
/// </summary>
/// <remarks>
/// The menu allows the user to select a new role from a predefined list, confirm the change,
/// and update the role in the system.
/// </remarks>
/// <param name="username">
/// The username of the user whose role is being changed.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user exits the role edit menu.
/// </param>
public class EditRole
{
    private readonly string _title;
    private readonly string _username;
    private readonly Action _onExit;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    internal EditRole(string username, Action onExit)
    {
        _title = "Edit Role";
        _username = username;
        _onExit = onExit;
        
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };
        
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "cancel" },
            { "Any other key", "select new role" }
        };
        
        Run();
    }

    /// <summary>
    /// Executes the main workflow for the role edit menu.
    /// Displays the user interface, handles user input for selecting a new role,
    /// and updates the role upon confirmation.
    /// </summary>
    /// <remarks>
    /// This method runs in a loop until the user exits the menu or successfully changes
    /// the role. It ensures proper validation of the selected role and handles errors gracefully.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown if the selected role is invalid.
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
                    Role newRole = GetRole();
                    
                    if(GetConfirm(ref running))
                    {
                        int userId = MenuHandler.db?.GetUserIdByUsername(_username) ?? 0;
                        MenuHandler.db?.UpdateUser(userId, null, newRole.ToString());
                        PasswordManager.ChangeUserRole(_username, newRole);
                        ConsoleOutput.PrintColorMessage($"Role successfully changed to {newRole}\n", ConsoleColor.Green);
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
    /// Prompts the user to select a new role from a predefined list of roles.
    /// Validates the input and returns the selected role.
    /// </summary>
    /// <returns>
    /// The role selected by the user.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the entered role is invalid (e.g., not within the allowed range).
    /// </exception>
    private Role GetRole()
    {
        while(true)
        {
            try
            {
                Console.WriteLine("Available roles:");
                Console.WriteLine("1. Administrator");
                Console.WriteLine("2. Warehouseman");
                Console.WriteLine("3. Logistician");
                Console.WriteLine("4. Warehouse Manager");
                int roleIndex = ConsoleInput.GetUserInt("Enter the role number (1-4): ");
                if (Enum.IsDefined(typeof(Role), roleIndex))
                {
                    return (Role)roleIndex;
                }
                else
                {
                    throw new ArgumentException("Invalid role. Please enter a number between 1 and 4.");
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

    /// <summary>
    /// Prompts the user to confirm the selected role.
    /// Updates the running flag based on the user's confirmation.
    /// </summary>
    /// <param name="running">
    /// A reference to a boolean flag indicating whether the process should continue running.
    /// This flag is set to false if the user confirms the role.
    /// </param>
    /// <returns>
    /// True if the user confirms the role, otherwise false.
    /// </returns>
    private bool GetConfirm(ref bool running)
    {
        Console.WriteLine("Is the role correct? (y/n): ");
        var key = ConsoleInput.GetConsoleKey();
        if (key == ConsoleKey.Y) 
        {
            running = false;
            return true;
        }
        else return false;   
    }

    /// <summary>
    /// Displays the user interface for the role edit menu.
    /// Shows the username being edited and provides navigation instructions for the user.
    /// </summary>
    /// <remarks>
    /// The method ensures proper formatting of the console output and clears the screen
    /// before displaying the content.
    /// </remarks>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = $"Change role for user: {_username}";
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
        
        Console.WriteLine(ConsoleOutput.HorizontalLine('-'));
    }
}
