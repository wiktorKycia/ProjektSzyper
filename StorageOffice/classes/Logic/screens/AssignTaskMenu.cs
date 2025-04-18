using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

public class AssignTaskMenu
{
    private readonly string _title;
    private readonly User _user;
    private readonly Action _onExit;
    private readonly CheckBoxSelect _select;
    private readonly List<database.Shipment> _shipments;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public AssignTaskMenu(User user, Action onExit)
    {
        _title = "Assign Tasks";
        _user = user;
        _onExit = onExit;
        _shipments = MenuHandler.db?.GetNotCompletedShipments() ?? new List<database.Shipment>();
        
        // Filter unassigned shipments only
        _shipments = _shipments.Where(s => s.UserId == null).ToList();
        
        if (!_shipments.Any())
        {
            var error = new Error("No unassigned shipments found.", () => onExit.Invoke());
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
            { ConsoleKey.A, AssignSelectedShipments },
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };
        
        _displayKeyboardActions = new Dictionary<string, string>()
        {
            { "\u2191", "move up" },
            { "\u2193", "move down" },
            { "<Enter>", "select" },
            { "A", "assign selected" },
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

    private void AssignSelectedShipments()
    {
        // Get selected shipments
        var selectedOptions = _select.CheckBoxOptions.Where(o => o.IsSelected).ToList();
        if (!selectedOptions.Any())
        {
            ConsoleOutput.PrintColorMessage("No shipments selected. Please select at least one shipment.\n", ConsoleColor.Yellow);
            Console.WriteLine("Press any key to continue...");
            ConsoleInput.WaitForAnyKey();
            return;
        }

        // Get selected shipment IDs
        var selectedIndices = selectedOptions.Select(o => _select.CheckBoxOptions.IndexOf(o)).ToList();
        var selectedShipments = selectedIndices.Select(i => _shipments[i]).ToList();

        // Open the user selection menu
        var userSelectMenu = new SelectUserForTaskMenu(selectedShipments, _onExit);
    }

    private void Display()
    {
        Console.Clear();
        string content = "Select shipments to assign:\n\n";
        
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
            content += ConsoleOutput.CenteredText("No unassigned shipments available", true);
        }
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
