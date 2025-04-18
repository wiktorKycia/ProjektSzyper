using System;
using StorageOffice.classes.CLI;

namespace StorageOffice.classes.Logic;

public class CompleteShipment
{
    private readonly database.Shipment _shipment;
    private readonly Action _onSuccess;
    private readonly Action _onCancel;
    private readonly string _title;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public CompleteShipment(database.Shipment shipment, Action onSuccess, Action onCancel)
    {
        _shipment = shipment;
        _onSuccess = onSuccess;
        _onCancel = onCancel;
        _title = $"Complete Shipment #{shipment.ShipmentId}";

        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>()
        {
            { ConsoleKey.Y, CompleteTheShipment },
            { ConsoleKey.N, () => _onCancel.Invoke() },
            { ConsoleKey.Escape, () => _onCancel.Invoke() }
        };

        _displayKeyboardActions = new Dictionary<string, string>()
        {
            { "Y", "confirm completion" },
            { "N", "cancel" },
            { "<Esc>", "cancel" }
        };

        Run();
    }

    private void Run()
    {
        Display();
        var key = ConsoleInput.GetConsoleKey();
        if (_keyboardActions.ContainsKey(key))
        {
            _keyboardActions[key]();
        }
        else
        {
            _onCancel.Invoke();
        }
    }

    private void CompleteTheShipment()
    {
        try
        {
            if (_shipment.ShipmentItems == null || !_shipment.ShipmentItems.Any())
            {
                ConsoleOutput.PrintColorMessage("Cannot complete a shipment with no items!\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                _onCancel.Invoke();
                return;
            }
            
            MenuHandler.db?.MarkShipmentAsDone(_shipment.ShipmentId);
            ConsoleOutput.PrintColorMessage("Shipment marked as complete successfully!\n", ConsoleColor.Green);
            Console.WriteLine("Press any key to continue...");
            ConsoleInput.WaitForAnyKey();
            _onSuccess.Invoke();
        }
        catch (Exception ex)
        {
            ConsoleOutput.PrintColorMessage($"Error: {ex.Message}\n", ConsoleColor.Red);
            Console.WriteLine("Press any key to continue...");
            ConsoleInput.WaitForAnyKey();
            _onCancel.Invoke();
        }
    }

    private void Display()
    {
        Console.Clear();
        string content = $"Are you sure you want to mark Shipment #{_shipment.ShipmentId} as complete?\n\n";
        
        content += $"Type: {_shipment.ShipmentType}\n";
        
        if (_shipment.ShipmentType == database.ShipmentType.Inbound)
        {
            content += $"Shipper: {_shipment.Shipper?.Name ?? "Not assigned"}\n";
        }
        else
        {
            content += $"Shop: {_shipment.Shop?.ShopName ?? "Not assigned"}\n";
        }
        
        content += "\nItems in this shipment:\n";
        
        if (_shipment.ShipmentItems != null && _shipment.ShipmentItems.Any())
        {
            string[] headers = { "Product", "Quantity", "Unit" };
            List<string[]> itemRows = new List<string[]>();
            
            foreach (var item in _shipment.ShipmentItems)
            {
                itemRows.Add(new string[] { 
                    item.Product.Name, 
                    item.Quantity.ToString(), 
                    item.Product.Unit 
                });
            }
            
            content += ConsoleOutput.WriteTable(itemRows, headers);
        }
        else
        {
            content += "No items in this shipment.";
        }
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
        
        Console.WriteLine(ConsoleOutput.HorizontalLine('-'));
    }
}
