using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

public class SelectUserForTaskMenu
{
    private readonly string _title;
    private readonly List<database.Shipment> _shipments;
    private readonly Action _onExit;
    private readonly RadioSelect _select;
    private readonly List<database.User> _warehousemen;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public SelectUserForTaskMenu(List<database.Shipment> shipments, Action onExit)
    {
        _title = "Assign to Warehouseman";
        _shipments = shipments;
        _onExit = onExit;
        
        // Get all users with Warehouseman role
        _warehousemen = MenuHandler.db?.GetAllUsers()
            .Where(u => u.Role == database.UserRole.Warehouseman)
            .ToList() ?? new List<database.User>();
        
        if (!_warehousemen.Any())
        {
            var error = new Error("No warehousemen available to assign tasks.", () => onExit.Invoke());
            return;
        }
        
        var options = _warehousemen.Select(u => new RadioOption(
            u.Username,
            () => AssignTasksToUser(u)
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
            { "<Enter>", "select" },
            { "<Esc>", "back" }
        };
        
        Run();
    }

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
                if (key == ConsoleKey.Escape || key == ConsoleKey.Enter)
                {
                    running = false;
                }
            }
        }
    }

    private void AssignTasksToUser(database.User user)
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
            var error = new Error($"Error assigning tasks: {ex.Message}", () => {});
        }
    }

    private void Display()
    {
        Console.Clear();
        string content = "Select a warehouseman to assign the task(s) to:\n\n";
        
        string[] shipmentHeaders = { "ID", "Type", "Source/Destination" };
        List<string[]> shipmentRows = new List<string[]>();
        
        foreach (var shipment in _shipments)
        {
            string sourceOrDest = shipment.ShipmentType == database.ShipmentType.Inbound ? 
                shipment.Shipper?.Name ?? "Unknown" : 
                shipment.Shop?.ShopName ?? "Unknown";
                
            shipmentRows.Add(new string[] { 
                shipment.ShipmentId.ToString(), 
                shipment.ShipmentType.ToString(), 
                sourceOrDest 
            });
        }
        
        content += "Selected shipments to assign:\n";
        content += ConsoleOutput.WriteTable(shipmentRows, shipmentHeaders);
        content += "\n";
        
        if (_select.Options != null)
        {
            foreach (var option in _select.Options)
            {
                content += ConsoleOutput.CenteredText(option.ToString() + "\n", true);
            }
        }
        else
        {
            content += ConsoleOutput.CenteredText("No warehousemen available", true);
        }
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
