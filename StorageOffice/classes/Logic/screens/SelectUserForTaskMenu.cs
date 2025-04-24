using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents a menu for selecting a warehouseman to assign shipments to.
/// </summary>
public class SelectUserForTaskMenu
{
    private readonly string _title;
    private readonly List<database.Shipment> _shipments;
    private readonly Action _onExit;
    private readonly RadioSelect _select;
    private readonly List<database.User> _users;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectUserForTaskMenu"/> class.
    /// </summary>
    /// <param name="shipments">The list of shipments to be assigned.</param>
    /// <param name="onExit">The action to execute when exiting the menu.</param>
    public SelectUserForTaskMenu(List<database.Shipment> shipments, Action onExit)
    {
        _title = "Select User For Task";
        _shipments = shipments;
        _onExit = onExit;

        // Get all users with the Warehouseman role
        _users = MenuHandler.db?.GetAllUsers().Where(u => u.Role == database.UserRole.Warehouseman).ToList()
            ?? new List<database.User>();

        if (!_users.Any())
        {
            var error = new Error("No warehousemen found to assign tasks to.", () => onExit.Invoke());
            return;
        }

        var options = _users.Select(u => new RadioOption(
            $"{u.Username} (Warehouseman)",
            () => GetConfirmation(u)
        )).ToList();

        _select = new RadioSelect(options);

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
            { "<Enter>", "select user" },
            { "<Esc>", "back" }
        };

        Run();
    }

    /// <summary>
    /// Runs the main loop for the menu, handling user input and actions.
    /// </summary>
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
                }
                else if (key == ConsoleKey.Enter)
                {
                    _keyboardActions[key]();
                    running = false;
                }
                else
                {
                    _keyboardActions[key]();
                }
            }
        }
    }

    private void GetConfirmation(database.User u)
    {
        Console.WriteLine("Are you sure you want to assign the selected shipments? (Y/N)");
        var key = ConsoleInput.GetConsoleKey();
        if (key == ConsoleKey.Y)
        {
            AssignShipmentsToUser(u);
        }
        else
        {
            _onExit.Invoke();
        }
    }

    /// <summary>
    /// Assigns the selected shipments to the specified user.
    /// </summary>
    /// <param name="user">The user to assign the shipments to.</param>
    private void AssignShipmentsToUser(database.User user)
    {
        try
        {
            foreach (var shipment in _shipments)
            {
                MenuHandler.db?.UpdateUserAssaignedToShipment(user.UserId, shipment.ShipmentId);
            }

            ConsoleOutput.PrintColorMessage($"Successfully assigned {_shipments.Count} shipment(s) to {user.Username}.\n", ConsoleColor.Green);
            Console.WriteLine("Press any key to continue...");
            ConsoleInput.WaitForAnyKey();
            _onExit.Invoke();
        }
        catch (Exception ex)
        {
            var error = new Error($"Error assigning shipments: {ex.Message}", () => _onExit.Invoke());
        }
    }

    /// <summary>
    /// Displays the menu, including the list of shipments and available warehousemen.
    /// </summary>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = $"Select a warehouseman to assign {_shipments.Count} shipment(s):\n\n";

        // Display shipments being assigned
        content += "Shipments to be assigned:\n";
        string[] headers = { "ID", "Type", "Source/Destination" };
        List<string[]> rows = new List<string[]>();

        foreach (var shipment in _shipments)
        {
            string sourceOrDest = shipment.ShipmentType == database.ShipmentType.Inbound ?
                shipment.Shipper?.Name ?? "Unknown" :
                shipment.Shop?.ShopName ?? "Unknown";

            rows.Add(new string[] {
                shipment.ShipmentId.ToString(),
                shipment.ShipmentType.ToString(),
                sourceOrDest
            });
        }

        content += ConsoleOutput.WriteTable(rows, headers) + "\n";

        // Display user options
        content += "Available warehousemen:\n";

        if (_select.Options != null)
        {
            foreach (var option in _select.Options)
            {
                content += "\n" + ConsoleOutput.CenteredText(option?.ToString() + "\n" ?? "[ ] No Text" + "\n", true);
            }
        }
        else
        {
            content += "\n" + ConsoleOutput.CenteredText("No warehousemen available", true);
        }

        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
