using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Models;
namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the Users screen, which displays a list of users and handles user interactions.
/// </summary>
class Users
{
    private readonly string _title;
    private readonly List<User> _users;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    /// <summary>
    /// Initializes a new instance of the <see cref="Users"/> class.
    /// </summary>
    /// <param name="users">The list of users to display.</param>
    /// <param name="onExit">The action to execute when the user exits the screen.</param>
    public Users(List<User> users, Action onExit)
    {
        _title = "Users";
        _users = users;
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, onExit.Invoke }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "back" }
        };
        Run();
    }

    /// <summary>
    /// Runs the main loop for the Users screen, handling user input and actions.
    /// </summary>
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
    /// Displays the Users screen, including the list of users and available keyboard actions.
    /// </summary>
    public void Display()
    {
        Console.Clear();
        string text = string.Empty;

        List<string[]> userData = [];
        foreach (var user in _users)
        {
            string[] data = [user.Username, user.Role.ToString()];
            userData.Add(data);
        }
        text += ConsoleOutput.WriteTable(userData, [ "Username", "Role" ]);

        text = string.Join(Environment.NewLine, text.Split(Environment.NewLine).Select(line => ConsoleOutput.CenteredText(line)));

        Console.WriteLine(ConsoleOutput.UIFrame(_title, text));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}