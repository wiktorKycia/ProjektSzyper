using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the menu for creating the first administrator user.
/// This class provides an interactive console-based workflow for entering the username
/// and password of the first administrator and saving the user in the system.
/// </summary>
/// <remarks>
/// The menu ensures proper validation of the entered username and password and handles
/// errors gracefully.
/// </remarks>
/// <param name="title">
/// The title of the menu.
/// </param>
/// <param name="heading">
/// The heading text to be displayed in the menu.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user creation process is completed successfully.
/// </param>
/// <param name="user">
/// The user object to be populated with the entered details.
/// </param>
public class FirstUser
{
    private readonly string _title;
    private readonly string _heading;
    private readonly Action _onExit;
    private readonly User _user;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    internal FirstUser(string title, string heading, Action onExit, User user)
    {
        _title = title;
        _heading = heading;
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, () => Environment.Exit(0) }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "exit" },
            { "Any other key", "enter username and password" }
        };
        _onExit = onExit;
        _user = user;
        Run();
    }

    /// <summary>
    /// Executes the main workflow for the first user creation menu.
    /// Displays the user interface, handles user input for entering the username and password,
    /// and saves the user upon confirmation.
    /// </summary>
    /// <remarks>
    /// This method runs in a loop until the user exits the menu or successfully creates
    /// the first administrator user.
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
            else
            {
                GetUsername(_user);
                string password = GetPassword();

                if(GetConfirm(ref running))
                {
                    PasswordManager.SaveNewUser(_user.Username, password, Role.Administrator);                    
                    MenuHandler.db?.AddUser(_user.Username, "Administrator");
                    
                    LogManager.AddNewLog($"Info: login of user {_user.Username} - successful");
                    ConsoleOutput.PrintColorMessage("User successfully created\n", ConsoleColor.Green);
                    Console.WriteLine("Press any key to continue...");
                    ConsoleInput.WaitForAnyKey();
                    _onExit.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Prompts the user to enter a username for the first administrator.
    /// Validates the input and updates the provided user object with the entered username.
    /// </summary>
    /// <param name="user">
    /// The user object to be updated with the entered username.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the entered username is null or empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the entered username is invalid (e.g., contains prohibited characters).
    /// </exception>
    private void GetUsername(User user)
    {
        bool isCorrect = false;
        while(!isCorrect)
        {
            try
            {
                user.Username = ConsoleInput.GetUserString("Enter the username of the first administrator: ");
                isCorrect = true;
            }
            catch (ArgumentNullException e)
            {
                ConsoleOutput.PrintColorMessage(e.Message + "\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
            catch (ArgumentException e)
            {
                ConsoleOutput.PrintColorMessage(e.Message + "\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
            catch (Exception e)
            {
                ConsoleOutput.PrintColorMessage($"An unexpected error occurred: {e.Message}\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
        }
    }

    /// <summary>
    /// Prompts the user to enter a password for the first administrator.
    /// Validates the input and returns the entered password.
    /// </summary>
    /// <returns>
    /// The validated password entered by the user.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the entered password is null or empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the entered password is invalid (e.g., does not meet complexity requirements).
    /// </exception>
    private string GetPassword()
    {
        while(true)
        {
            try
            {
                string password = ConsoleInput.GetUserString("Enter the password of the first administrator: ");
                return password;
            }
            catch (ArgumentNullException e)
            {
                ConsoleOutput.PrintColorMessage(e.Message + "\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
            catch (ArgumentException e)
            {
                ConsoleOutput.PrintColorMessage(e.Message + "\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
            catch (Exception e)
            {
                ConsoleOutput.PrintColorMessage($"An unexpected error occurred: {e.Message}\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
            }
        }
    }

    /// <summary>
    /// Prompts the user to confirm the entered details.
    /// Updates the running flag based on the user's confirmation.
    /// </summary>
    /// <param name="running">
    /// A reference to a boolean flag indicating whether the process should continue running.
    /// This flag is set to false if the user confirms the details.
    /// </param>
    /// <returns>
    /// True if the user confirms the details, otherwise false.
    /// </returns>
    private bool GetConfirm(ref bool running)
    {
        string answer = ConsoleInput.GetUserString("Is the information correct? (y/n): ");
        if (answer.ToLower() == "y")
        {
            running = false;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Displays the user interface for the first user creation menu.
    /// Shows the heading text and provides navigation instructions for the user.
    /// </summary>
    /// <remarks>
    /// The method ensures proper formatting of the console output and clears the screen
    /// before displaying the content.
    /// </remarks>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = _heading;
        // string content = CenteredText(_heading, true) + "\n" + "No user has been created on the system, so the details currently provided will be used to create the first administrator. Enter the details for him/her.\n";
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content.RightMargin()));
        
        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }

        Console.WriteLine(ConsoleOutput.HorizontalLine('-'));

    }
}