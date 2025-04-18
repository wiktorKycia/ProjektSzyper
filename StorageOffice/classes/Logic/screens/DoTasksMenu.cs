using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

public class DoTasksMenu
{
    private readonly string _title;
    private readonly User _user;
    private readonly Action _onExit;
    private readonly CheckBoxSelect _select;
    private readonly List<database.Shipment> _shipments;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public DoTasksMenu(User user, Action onExit)
    {
        _title = "Do Tasks";
        _user = user;
        _onExit = onExit;
        
        // Get user ID from database
        int userId;
        try
        {
            userId = MenuHandler.db?.GetUserIdByUsername(user.Username) ?? 0;
            if (userId == 0) throw new InvalidOperationException("User not found in database.");
        }
        catch (Exception ex)
        {
            var error = new Error($"Error: {ex.Message}", () => onExit.Invoke());
            return;
        }
        
        // Get assigned, not completed shipments for this user
        _shipments = MenuHandler.db?.GetNotCompletedShipmentsAssignedToUser(userId) ?? new List<database.Shipment>();
        
        if (!_shipments.Any())
        {
            var error = new Error("No tasks assigned to you.", () => onExit.Invoke());
            return;
        }
        
        var options = _shipments.Select(s => new CheckBoxOption(
            $"ID: {s.ShipmentId} - {s.ShipmentType} - {(s.ShipmentType == database.ShipmentType.Inbound ? s.Shipper?.Name : s.Shop?.ShopName)}",
            () => {}
        )).ToList();
        
        _select = new CheckBoxSelect(options);
        
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>()
        {
            { ConsoleKey.UpArrow, _select.MoveUp },
            { ConsoleKey.DownArrow, _select.MoveDown },
            { ConsoleKey.Enter, _select.SelectOption },
            { ConsoleKey.D, CompleteSelectedTasks },
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };
        
        _displayKeyboardActions = new Dictionary<string, string>()
        {
            { "\u2191", "move up" },
            { "\u2193", "move down" },
            { "<Enter>", "select" },
            { "D", "complete selected" },
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
                if (key == ConsoleKey.Escape)
                {
                    running = false;
                }
                else
                {
                    _keyboardActions[key]();
                }
            }
        }
    }

    private void CompleteSelectedTasks()
    {
        // Get selected shipments
        var selectedOptions = _select.CheckBoxOptions.Where(o => o.IsSelected).ToList();
        if (!selectedOptions.Any())
        {
            ConsoleOutput.PrintColorMessage("No tasks selected. Please select at least one task.\n", ConsoleColor.Yellow);
            Console.WriteLine("Press any key to continue...");
            ConsoleInput.WaitForAnyKey();
            return;
        }

        // Get selected shipment IDs
        var selectedIndices = selectedOptions.Select(o => _select.CheckBoxOptions.IndexOf(o)).ToList();
        var selectedShipments = selectedIndices.Select(i => _shipments[i]).ToList();

        // Confirm completion
        Console.Clear();
        Console.WriteLine(ConsoleOutput.UIFrame("Confirm Task Completion", 
            "Are you sure you want to mark the selected tasks as completed? (Y/N)\n" +
            "This will update inventory levels and cannot be undone."));
        
        var key = ConsoleInput.GetConsoleKey();
        if (key == ConsoleKey.Y)
        {
            try
            {
                foreach (var shipment in selectedShipments)
                {
                    MenuHandler.db?.MarkShipmentAsDone(shipment.ShipmentId);
                }
                
                ConsoleOutput.PrintColorMessage($"Successfully completed {selectedShipments.Count} task(s).\n", ConsoleColor.Green);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                _onExit.Invoke();
            }
            catch (Exception ex)
            {
                var error = new Error($"Error completing tasks: {ex.Message}", () => {});
            }
        }
    }

    private void Display()
    {
        Console.Clear();
        string content = "Select tasks to complete:\n\n";
        
        if (_select.Options != null && _select.Options.Any())
        {
            for (int i = 0; i < _select.Options.Count(); i++)
            {
                var option = _select.Options.ElementAt(i);
                string optionText = option.ToString() ?? "[ ] No Text";
                
                if (_select.CheckBoxOptions[i].IsHighlighted)
                {
                    ConsoleOutput.PrintColorMessage(ConsoleOutput.CenteredText(optionText + "\n", true), ConsoleColor.Blue);
                    
                    // Display shipment items for the highlighted shipment
                    var shipment = _shipments[i];
                    content += "\nItems in highlighted shipment:\n";
                    
                    if (shipment.ShipmentItems != null && shipment.ShipmentItems.Any())
                    {
                        string[] headers = { "Product", "Quantity", "Unit" };
                        List<string[]> rows = new List<string[]>();
                        
                        foreach (var item in shipment.ShipmentItems)
                        {
                            rows.Add(new string[] { item.Product.Name, item.Quantity.ToString(), item.Product.Unit });
                        }
                        
                        content += ConsoleOutput.WriteTable(rows, headers);
                    }
                    else
                    {
                        content += "No items in this shipment.\n";
                    }
                }
                else
                {
                    Console.WriteLine(ConsoleOutput.CenteredText(optionText + "\n", true));
                }
            }
        }
        else
        {
            content += ConsoleOutput.CenteredText("No tasks assigned to you", true);
        }
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
