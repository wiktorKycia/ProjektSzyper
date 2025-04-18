using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

public class EditShipment
{
    private readonly string _title;
    private readonly database.Shipment _shipment;
    private readonly Action _onExit;
    private readonly User _user;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;
    private readonly Select _select;

    internal EditShipment(database.Shipment shipment, Select select, Action onExit, User user)
    {
        _title = "Edit Shipment";
        _shipment = shipment;
        _onExit = onExit;
        _user = user;
        _select = select;

        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.UpArrow, _select.MoveUp },
            { ConsoleKey.DownArrow, _select.MoveDown },
            { ConsoleKey.Enter, _select.InvokeOperation },
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
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
        while (running)
        {
            Display();
            var key = ConsoleInput.GetConsoleKey();
            if (_keyboardActions.ContainsKey(key))
            {
                if (key == ConsoleKey.Escape)
                {
                    running = false;
                    _onExit.Invoke();
                }
                else
                {
                    _keyboardActions[key]();
                }
            }
        }
    }

    private void Display()
    {
        Console.Clear();
        string shipmentInfo = $"Shipment ID: {_shipment.ShipmentId}\n" +
                             $"Type: {_shipment.ShipmentType}\n" +
                             $"Status: {(_shipment.IsCompleted ? "Completed" : "Pending")}\n";

        if (_shipment.ShipmentType == database.ShipmentType.Inbound)
        {
            shipmentInfo += $"Shipper: {_shipment.Shipper?.Name ?? "Not assigned"}\n";
        }
        else
        {
            shipmentInfo += $"Shop: {_shipment.Shop?.ShopName ?? "Not assigned"}\n";
        }

        string content = shipmentInfo + "\nSelect an option:\n";
        
        foreach (var option in _select.Options)
        {
            content += "\n" + ConsoleOutput.CenteredText(option?.ToString() + "\n" ?? "[ ] No Text" + "\n", true);
        }

        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
