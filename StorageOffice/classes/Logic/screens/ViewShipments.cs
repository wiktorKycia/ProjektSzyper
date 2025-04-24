using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.database;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the View Shipments screen, which displays a paginated list of shipments
/// and allows navigation between pages or filtering based on shipment status.
/// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewShipments"/> class.
    /// </summary>
    /// <param name="shipments">The list of shipments to display.</param>
    /// <param name="onExit">The action to execute when exiting the screen.</param>
    /// <param name="title">The title of the screen. Defaults to "View Shipments".</param>
    /// <param name="showCompletedOnly">Whether to show only completed shipments. Defaults to false.</param>
    /// <param name="showPendingOnly">Whether to show only pending shipments. Defaults to false.</param>
    internal ViewShipments(List<Shipment> shipments, Action onExit, string title = "View Shipments",
        bool showCompletedOnly = false, bool showPendingOnly = false)
    {
        _title = title;
        _shipments = shipments;
        _onExit = onExit;
        _showCompletedOnly = showCompletedOnly;
        _showPendingOnly = showPendingOnly;

        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>()
            {
                { ConsoleKey.Escape, () => onExit.Invoke() },
                { ConsoleKey.LeftArrow, PreviousPage },
                { ConsoleKey.RightArrow, NextPage }
            };
        _displayKeyboardActions = new Dictionary<string, string>()
            {
                { "\u2190", "previous page" },
                { "\u2192", "next page" },
                { "<Esc>", "back" }
            };
        Run();
    }

    /// <summary>
    /// Runs the main loop for the View Shipments screen, handling user input and actions.
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
                else
                {
                    _keyboardActions[key]();
                }
            }
        }
    }

    /// <summary>
    /// Navigates to the previous page of shipments, if available.
    /// </summary>
    private void PreviousPage()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
        }
    }

    /// <summary>
    /// Navigates to the next page of shipments, if available.
    /// </summary>
    private void NextPage()
    {
        int totalPages = (_shipments.Count - 1) / _itemsPerPage + 1;
        if (_currentPage < totalPages - 1)
        {
            _currentPage++;
        }
    }

    /// <summary>
    /// Displays the current page of shipments, including shipment details and items.
    /// </summary>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
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

            // Display shipments in a table format
            string[] headers = { "ID", "Type", "Status", "From/To", "Handler", "Date" };
            List<string[]> shipmentRows = new List<string[]>();

            foreach (var shipment in pageShipments)
            {
                string fromTo = shipment.ShipmentType == ShipmentType.Inbound ?
                    shipment.Shipper?.Name ?? "Unknown" :
                    shipment.Shop?.ShopName ?? "Unknown";

                shipmentRows.Add(new string[] {
                        shipment.ShipmentId.ToString(),
                        shipment.ShipmentType.ToString(),
                        shipment.IsCompleted ? "Completed" : "Pending",
                        fromTo,
                        shipment.User?.Username ?? "Unassigned",
                        shipment.ShippedDate?.ToShortDateString() ?? "Not shipped"
                    });
            }

            content += ConsoleOutput.WriteTable(shipmentRows, headers);

            // For each shipment, show its items
            foreach (var shipment in pageShipments)
            {
                content += $"\nShipment #{shipment.ShipmentId} Items:\n";

                if (shipment.ShipmentItems == null || !shipment.ShipmentItems.Any())
                {
                    content += "  - No items\n";
                }
                else
                {
                    string[] itemHeaders = { "Product", "Quantity", "Unit" };
                    List<string[]> itemRows = new List<string[]>();

                    foreach (var item in shipment.ShipmentItems)
                    {
                        itemRows.Add(new string[] {
                                item.Product.Name,
                                item.Quantity.ToString(),
                                item.Product.Unit
                            });
                    }

                    content += ConsoleOutput.WriteTable(itemRows, itemHeaders);
                }

                content += "\n" + ConsoleOutput.HorizontalLine('-') + "\n";
            }
        }

        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
