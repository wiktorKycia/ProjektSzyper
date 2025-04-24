using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Models;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the menu for performing tasks assigned to a user.
/// This class provides an interactive console-based workflow for viewing, selecting,
/// and completing tasks (shipments) assigned to the current user.
/// </summary>
/// <remarks>
/// The menu displays tasks in a grid layout, allows navigation and selection,
/// and facilitates the completion of selected tasks.
/// </remarks>
/// <param name="user">
/// The user interacting with the menu.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user exits the menu.
/// </param>
public class DoTasksMenu
{
    private readonly string _title;
    private readonly User _user;
    private readonly Action _onExit;
    private readonly CheckBoxSelect _select;
    private readonly List<database.Shipment> _shipments;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;
    private int _optionsPerRow; // Calculated based on console width
    private const int OPTION_MIN_WIDTH = 15; // Minimum width for an option
    private const int OPTION_MAX_WIDTH = 25; // Maximum width for an option
    private const int OPTION_PADDING = 2; // Padding between options

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
            $"ID: {s.ShipmentId,2} - {s.ShipmentType.ToString().Substring(0, 3)}",
            () => {}
        )).ToList();
        
        _select = new CheckBoxSelect(options);
        
        // Calculate optimal number of options per row based on console width
        CalculateOptionsPerRow();
        
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>()
        {
            { ConsoleKey.UpArrow, MoveUp },
            { ConsoleKey.DownArrow, MoveDown },
            { ConsoleKey.LeftArrow, MoveLeft },
            { ConsoleKey.RightArrow, MoveRight },
            { ConsoleKey.Enter, _select.SelectOption },
            { ConsoleKey.C, CompleteSelectedTasks },
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };
        
        _displayKeyboardActions = new Dictionary<string, string>()
        {
            { "\u2190\u2191\u2192\u2193", "navigate" },
            { "<Enter>", "select" },
            { "<C>", "complete selected" },
            { "<Esc>", "back" }
        };
        
        Run();
    }

    /// <summary>
    /// Executes the main workflow for the task menu.
    /// Displays the user interface, handles user input for navigation and selection,
    /// and processes the completion of selected tasks.
    /// </summary>
    /// <remarks>
    /// This method runs in a loop until the user exits the menu. It ensures proper handling
    /// of keyboard actions and updates the display accordingly.
    /// </remarks>
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

    /// <summary>
    /// Calculates the optimal number of options to display per row based on the console width.
    /// Adjusts the layout dynamically to ensure proper spacing and alignment of options.
    /// </summary>
    /// <remarks>
    /// The calculation considers the minimum and maximum width of options, as well as padding
    /// between options, to determine the number of options that can fit in a single row.
    /// </remarks>
    private void CalculateOptionsPerRow()
    {
        int effectiveWidth = ConsoleOutput.UIWidth - 10; // Leave some margin
        int optionWidth = Math.Min(OPTION_MAX_WIDTH, Math.Max(OPTION_MIN_WIDTH, 
            _shipments.Max(s => $"ID: {s.ShipmentId} - {s.ShipmentType}".Length) + OPTION_PADDING));
        
        _optionsPerRow = Math.Max(1, effectiveWidth / (optionWidth + OPTION_PADDING));
    }

    /// <summary>
    /// Moves the selection highlight up by one row in the grid layout.
    /// Ensures that the selection remains within the bounds of the available options.
    /// </summary>
    private void MoveUp()
    {
        int currentIndex = GetHighlightedIndex();
        if (currentIndex >= _optionsPerRow)
        {
            // Move up to the previous row
            for (int i = 0; i < _optionsPerRow; i++)
            {
                _select.MoveUp();
            }
        }
    }

    /// <summary>
    /// Moves the selection highlight down by one row in the grid layout.
    /// Ensures that the selection remains within the bounds of the available options.
    /// </summary>
    private void MoveDown()
    {
        int currentIndex = GetHighlightedIndex();
        int rowsBelow = (_select.CheckBoxOptions.Count - currentIndex - 1) / _optionsPerRow;
        
        if (rowsBelow > 0)
        {
            // Move down to the next row
            int stepsDown = Math.Min(_optionsPerRow, _select.CheckBoxOptions.Count - currentIndex - 1);
            for (int i = 0; i < stepsDown; i++)
            {
                _select.MoveDown();
            }
        }
    }

    /// <summary>
    /// Moves the selection highlight left by one option within the same row.
    /// Ensures that the selection does not move out of the row's bounds.
    /// </summary>
    private void MoveLeft()
    {
        int currentIndex = GetHighlightedIndex();
        if (currentIndex % _optionsPerRow > 0)
        {
            // Move left within the same row
            _select.MoveUp();
        }
    }

    /// <summary>
    /// Moves the selection highlight right by one option within the same row.
    /// Ensures that the selection does not move out of the row's bounds.
    /// </summary>
    private void MoveRight()
    {
        int currentIndex = GetHighlightedIndex();
        if (currentIndex % _optionsPerRow < _optionsPerRow - 1 && currentIndex < _select.CheckBoxOptions.Count - 1)
        {
            // Move right within the same row
            _select.MoveDown();
        }
    }

    /// <summary>
    /// Retrieves the index of the currently highlighted option in the grid layout.
    /// </summary>
    /// <returns>
    /// The index of the highlighted option, or 0 if no option is highlighted.
    /// </returns>
    private int GetHighlightedIndex()
    {
        for (int i = 0; i < _select.CheckBoxOptions.Count; i++)
        {
            if (_select.CheckBoxOptions[i].IsHighlighted)
            {
                return i;
            }
        }
        return 0;
    }

    /// <summary>
    /// Marks the selected tasks (shipments) as completed.
    /// Updates the database to reflect the completion of the selected tasks and displays
    /// a confirmation message to the user.
    /// </summary>
    /// <remarks>
    /// If no tasks are selected, a warning message is displayed, and the method exits.
    /// If the user confirms the completion, the selected tasks are marked as completed.
    /// </remarks>
    /// <exception cref="Exception">
    /// Catches and handles any exceptions that occur during the completion process,
    /// displaying an error message to the user.
    /// </exception>
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

    /// <summary>
    /// Displays the user interface for the task menu.
    /// Shows the available tasks in a grid layout, highlights the selected option,
    /// and displays details of the highlighted task.
    /// </summary>
    /// <remarks>
    /// The method dynamically adjusts the layout based on the console width and provides
    /// navigation instructions for the user.
    /// </remarks>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine(ConsoleOutput.Header(_title));
        string content = "Select tasks to complete:\n\n";
        
        // Display options in a horizontal grid layout
        if (_select.Options != null && _select.Options.Any())
        {
            // Recalculate columns in case console was resized
            CalculateOptionsPerRow();
            
            // Build and display grid
            for (int i = 0; i < _select.Options.Count(); i += _optionsPerRow)
            {
                // For each row
                for (int j = 0; j < _optionsPerRow && i + j < _select.Options.Count(); j++)
                {
                    int optionIndex = i + j;
                    var option = _select.Options.ElementAt(optionIndex);
                    string optionText = option.ToString() ?? "[ ] No Text";
                    
                    // Calculate optimal option width for display
                    int optionWidth = Math.Min(OPTION_MAX_WIDTH, Math.Max(OPTION_MIN_WIDTH, optionText.Length + OPTION_PADDING));
                    
                    // Truncate if needed and ensure consistent width
                    if (optionText.Length > optionWidth - OPTION_PADDING)
                    {
                        optionText = ConsoleOutput.Truncate(optionText, optionWidth - OPTION_PADDING);
                    }
                    optionText = optionText.PadRight(optionWidth);
                    
                    // Print the option with color highlighting for the currently selected one
                    if (_select.CheckBoxOptions[optionIndex].IsHighlighted)
                    {
                        ConsoleOutput.PrintColorMessage("▶ " + optionText + " ◀", ConsoleColor.Cyan);
                    }
                    else
                    {
                        Console.Write("  " + optionText + "  ");
                    }
                }
                Console.WriteLine(); // New line after each row
            }
            
            // Display details for the highlighted shipment
            int highlightedIndex = GetHighlightedIndex();
            var highlightedShipment = _shipments[highlightedIndex];
            
            content += "\n" + ConsoleOutput.HorizontalLine('-') + "\n";
            content += $"Details for highlighted shipment:\n";
            ConsoleOutput.PrintColorMessage($"ID: {highlightedShipment.ShipmentId} - ", ConsoleColor.Yellow);
            
            // Display source/destination
            if (highlightedShipment.ShipmentType == database.ShipmentType.Inbound)
            {
                Console.WriteLine($"Inbound from {highlightedShipment.Shipper?.Name ?? "Unknown"}");
            }
            else
            {
                Console.WriteLine($"Outbound to {highlightedShipment.Shop?.ShopName ?? "Unknown"}");
            }
            
            // Display shipment items
            Console.WriteLine("\nItems in shipment:");
            
            if (highlightedShipment.ShipmentItems != null && highlightedShipment.ShipmentItems.Any())
            {
                string[] headers = { "Product", "Quantity", "Unit" };
                List<string[]> rows = new List<string[]>();
                
                foreach (var item in highlightedShipment.ShipmentItems)
                {
                    rows.Add([item.Product.Name, item.Quantity.ToString(), item.Product.Unit]);
                }
                
                Console.Write(ConsoleOutput.WriteTable(rows, headers));
            }
            else
            {
                Console.WriteLine("No items in this shipment.");
            }
        }
        else
        {
            content += ConsoleOutput.CenteredText("No tasks assigned to you", true);
            Console.WriteLine(content);
        }
        
        Console.WriteLine(ConsoleOutput.HorizontalLine('='));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
