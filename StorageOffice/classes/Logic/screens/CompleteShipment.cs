using System;
using StorageOffice.classes.CLI;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the logic for completing a shipment.
/// This class provides an interactive console-based workflow for marking a shipment as complete,
/// including displaying shipment details and handling user confirmation.
/// </summary>
/// <remarks>
/// The class ensures that only shipments with items can be marked as complete and interacts
/// with the database to update the shipment's status.
/// </remarks>
/// <param name="shipment">
/// The shipment to be marked as complete.
/// </param>
/// <param name="onSuccess">
/// An action to be invoked when the shipment is successfully marked as complete.
/// </param>
/// <param name="onCancel">
/// An action to be invoked when the user cancels the completion process.
/// </param>
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

    /// <summary>
    /// Executes the main workflow for completing a shipment.
    /// Displays the shipment details and handles user input for confirming or canceling the completion process.
    /// </summary>
    /// <remarks>
    /// This method ensures that the appropriate action is invoked based on the user's input.
    /// </remarks>
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

    /// <summary>
    /// Marks the shipment as complete.
    /// Validates that the shipment contains items before updating its status in the database.
    /// </summary>
    /// <remarks>
    /// If the shipment is successfully marked as complete, the success action is invoked.
    /// If the shipment has no items or an error occurs, the cancel action is invoked.
    /// </remarks>
    /// <exception cref="Exception">
    /// Catches and handles any exceptions that occur during the completion process,
    /// displaying an error message to the user.
    /// </exception>
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

    /// <summary>
    /// Displays the user interface for completing a shipment.
    /// Shows the shipment details, including its type, associated shipper or shop, and items.
    /// Provides navigation instructions for confirming or canceling the completion process.
    /// </summary>
    /// <remarks>
    /// The method dynamically builds the content to display shipment details and ensures
    /// proper formatting for the console output.
    /// </remarks>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
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
