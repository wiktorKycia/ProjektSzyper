using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.database;

namespace StorageOffice.classes.Logic;

public class ViewShipments
{
    private readonly string _title;
    private readonly List<Shipment> _shipments;
    private readonly Action _onExit;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;
    private int _currentPage = 0;
    private readonly int _itemsPerPage = 5;
    private readonly bool _showCompletedOnly;
    private readonly bool _showPendingOnly;

    internal ViewShipments(List<Shipment> shipments, Action onExit, string title = "View Shipments", 
        bool showCompletedOnly = false, bool showPendingOnly = false)
    {
        _title = title;
        _shipments = shipments;
        _onExit = onExit;
        _showCompletedOnly = showCompletedOnly;
        _showPendingOnly = showPendingOnly;

        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, () => onExit.Invoke() },
            { ConsoleKey.LeftArrow, PreviousPage },
            { ConsoleKey.RightArrow, NextPage }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "\u2190", "previous page" },
            { "\u2192", "next page" },
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
                }
                else
                {
                    _keyboardActions[key]();
                }
            }
        }
    }

    private void PreviousPage()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
        }
    }

    private void NextPage()
    {
        int totalPages = (_shipments.Count - 1) / _itemsPerPage + 1;
        if (_currentPage < totalPages - 1)
        {
            _currentPage++;
        }
    }

    private void Display()
    {
        Console.Clear();
        string content = $"{_title}\n\n";

        // Filter shipments based on settings
        var filteredShipments = _shipments;
        if (_showCompletedOnly)
        {
            filteredShipments = _shipments.Where(s => s.IsCompleted).ToList();
        }
        else if (_showPendingOnly)
        {
            filteredShipments = _shipments.Where(s => !s.IsCompleted).ToList();
        }

        if (!filteredShipments.Any())
        {
            content += "No shipments found.";
        }
        else
        {
            content += $"Showing page {_currentPage + 1} of {(filteredShipments.Count - 1) / _itemsPerPage + 1}\n\n";

            // Get current page items
            var pageShipments = filteredShipments
                .Skip(_currentPage * _itemsPerPage)
                .Take(_itemsPerPage)
                .ToList();

            foreach (var shipment in pageShipments)
            {
                content += $"ID: {shipment.ShipmentId}\n";
                content += $"Type: {shipment.ShipmentType}\n";
                content += $"Status: {(shipment.IsCompleted ? "Completed" : "Pending")}\n";
                
                if (shipment.ShipmentType == ShipmentType.Inbound)
                {
                    content += $"Shipper: {shipment.Shipper?.Name ?? "Not assigned"}\n";
                }
                else
                {
                    content += $"Shop: {shipment.Shop?.ShopName ?? "Not assigned"}\n";
                }
                
                content += $"Handled by: {shipment.User?.Username ?? "Not assigned"}\n";
                
                if (shipment.ShippedDate.HasValue)
                {
                    content += $"Shipped date: {shipment.ShippedDate.Value.ToShortDateString()}\n";
                }
                
                content += "Items:\n";
                if (shipment.ShipmentItems != null && shipment.ShipmentItems.Any())
                {
                    foreach (var item in shipment.ShipmentItems)
                    {
                        content += $"  - {item.Product.Name}: {item.Quantity} {item.Product.Unit}\n";
                    }
                }
                else
                {
                    content += "  - No items\n";
                }
                content += "--------------------------------\n";
            }
        }

        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
