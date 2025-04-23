using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the logic for adding a new user to the system.
/// This class provides an interactive console-based workflow for creating a user,
/// including setting their username, password, and role.
/// </summary>
/// <remarks>
/// The class is designed to be used in a console application and ensures proper
/// validation of user input during the creation process.
/// </remarks>
public class AddUser
{
    private readonly string _title;
    private readonly string _heading;
    private readonly Action _backMenu;
    private readonly User _user;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    /// <param name="backMenu">
    /// An action to be invoked when the user exits the user creation process.
    /// </param>
    /// <param name="user">
    /// The user object to be populated with the new user's details.
    /// </param>
    internal AddUser(Action backMenu, User user)
    {
        _title = "Add user";
        _heading = "Add user";
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, () => backMenu.Invoke() }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "exit" },
            { "Any other key", "enter username and password" }
        };
        _backMenu = backMenu;
        _user = user;
        Run();
    }

    /// <summary>
    /// Executes the main workflow for adding a new user.
    /// Displays the user interface, handles user input for username, password, and role,
    /// and saves the new user to the database upon confirmation.
    /// </summary>
    /// <remarks>
    /// This method runs in a loop until the user creation process is completed or canceled.
    /// It ensures proper validation of all inputs and handles errors gracefully.
    /// </remarks>
    /// <exception cref="Exception">
    /// Catches and handles any exceptions that occur during the user creation process,
    /// ensuring the application remains stable.
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
            }
            else
            {
                GetUsername(_user);
                string password = GetPassword();
                Role role = GetRole();

                if(GetConfirm(ref running))
                {
                    PasswordManager.SaveNewUser(_user.Username, password, role);
                    MenuHandler.db?.AddUser(_user.Username, role.ToString());
                    ConsoleOutput.PrintColorMessage("User successfully created!\n", ConsoleColor.Green);
                    Console.WriteLine("Press any key to continue...");
                    ConsoleInput.WaitForAnyKey();
                    _backMenu.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Prompts the user to enter a username and validates the input.
    /// Updates the provided user object with the entered username.
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
                user.Username = ConsoleInput.GetUserString("Enter the username: ");
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
    /// Prompts the user to enter a password and validates the input.
    /// Returns the entered password if it is valid.
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
                string password = ConsoleInput.GetUserString("Enter the password: ");
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
    /// Prompts the user to select a role for the new user from a predefined list of roles.
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
                Console.WriteLine("1. Administrator");
                Console.WriteLine("2. Warehouseman");
                Console.WriteLine("3. Logistician");
                Console.WriteLine("4. Warehouse Manager");
                int roleIndex = ConsoleInput.GetUserInt("Enter the role: ");
                if (Enum.IsDefined(typeof(Role), roleIndex))
                {
                    return (Role)roleIndex;
                }
                else
                {
                    throw new ArgumentException("Invalid role. Please enter a number between 1 and 4.");
                }
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
    /// Displays the user interface for the AddUser screen.
    /// Clears the console, sets up the heading and title, and displays
    /// available keyboard actions for the user.
    /// </summary>
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