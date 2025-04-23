using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the login menu for authenticating users.
/// This class provides an interactive console-based workflow for entering the username
/// and password and verifying the user's credentials.
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
/// <param name="nextMenu">
/// An action to be invoked when the user is successfully authenticated.
/// </param>
/// <param name="user">
/// The user object to be populated with the authenticated user's details.
/// </param>
public class Login
{
    private readonly string _title;
    private readonly string _heading;
    private readonly Action _nextMenu;
    private readonly User _user;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    internal Login(string title, string heading, Action nextMenu, User user)
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
        _nextMenu = nextMenu;
        _user = user;
        Run();
    }

    /// <summary>
    /// Executes the main workflow for the login menu.
    /// Displays the user interface, handles user input for entering the username and password,
    /// and verifies the user's credentials.
    /// </summary>
    /// <remarks>
    /// This method runs in a loop until the user exits the menu or successfully logs in.
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

                Role? role = PasswordManager.VerifyPasswordAndGetRole(_user.Username, password);
                if (role == null)
                {
                    Console.WriteLine("Username or password is incorrect. Press any key and try again");
                    Console.ReadKey();
                    continue;
                }
                else
                {
                    _user.Role = (Role)role;
                    _nextMenu.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Prompts the user to enter their username.
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
                user.Username = ConsoleInput.GetUserString("Enter your username: ");
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
    /// Prompts the user to enter their password.
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
                string password = ConsoleInput.GetUserString("Enter your password: ");
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
    /// Displays the user interface for the login menu.
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
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content.RightMargin()));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }

        Console.WriteLine(ConsoleOutput.HorizontalLine('-'));

    }
}