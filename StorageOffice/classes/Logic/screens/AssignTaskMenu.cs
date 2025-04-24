using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Models;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the menu for assigning tasks to users.
/// This class provides an interactive console-based workflow for selecting unassigned shipments
/// and assigning them to a user.
/// </summary>
/// <remarks>
/// The menu displays unassigned shipments in a grid layout, allows navigation and selection,
/// and facilitates the assignment of selected shipments to a user.
/// </remarks>
public class AssignTaskMenu
{
    private readonly string _title;
    private readonly User _user;
    private readonly Action _onExit;
    private readonly CheckBoxSelect _select;
    private readonly List<database.Shipment> _shipments;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;
    private int _optionsPerRow; // Calculated based on console width
    private const int OPTION_MIN_WIDTH = 25; // Minimum width for an option
    private const int OPTION_MAX_WIDTH = 40; // Maximum width for an option
    private const int OPTION_PADDING = 3; // Padding between options

    /// <param name="user">
    /// The user performing the task assignment.
    /// </param>
    /// <param name="onExit">
    /// An action to be invoked when the user exits the task assignment menu.
    /// </param>
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
        
        // Calculate optimal number of options per row based on console width
        CalculateOptionsPerRow();
        
        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>()
        {
            { ConsoleKey.UpArrow, MoveUp },
            { ConsoleKey.DownArrow, MoveDown },
            { ConsoleKey.LeftArrow, MoveLeft },
            { ConsoleKey.RightArrow, MoveRight },
            { ConsoleKey.Enter, _select.SelectOption },
            { ConsoleKey.A, AssignSelectedShipments },
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };
        
        _displayKeyboardActions = new Dictionary<string, string>()
        {
            { "\u2190\u2191\u2192\u2193", "navigate" },
            { "<Enter>", "select" },
            { "A", "assign selected" },
            { "<Esc>", "back" }
        };
        
        Run();
    }

    /// <summary>
    /// Executes the main workflow for the task assignment menu.
    /// Displays the user interface, handles user input for navigation and selection,
    /// and processes the assignment of selected shipments.
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
        int effectiveWidth = ConsoleOutput.UIWidth - 8; // Leave some margin
        int maxOptionTextLength = _shipments.Max(s => 
            $"ID: {s.ShipmentId} - {s.ShipmentType} - {(s.ShipmentType == database.ShipmentType.Inbound ? s.Shipper?.Name : s.Shop?.ShopName)}".Length);
        
        int optionWidth = Math.Min(OPTION_MAX_WIDTH, Math.Max(OPTION_MIN_WIDTH, maxOptionTextLength + OPTION_PADDING));
        
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
    /// Assigns the selected shipments to a user.
    /// Opens a user selection menu to choose the user for the assignment.
    /// </summary>
    /// <remarks>
    /// If no shipments are selected, a warning message is displayed, and the method exits.
    /// Otherwise, the selected shipments are passed to the user selection menu for assignment.
    /// </remarks>

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

    /// <summary>
    /// Displays the user interface for the task assignment menu.
    /// Shows the available shipments in a grid layout, highlights the selected option,
    /// and displays details of the highlighted shipment.
    /// </summary>
    /// <remarks>
    /// The method dynamically adjusts the layout based on the console width and provides
    /// navigation instructions for the user.
    /// </remarks>

    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        Console.WriteLine(ConsoleOutput.Header(_title));
        
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
            
            Console.WriteLine("\n" + ConsoleOutput.HorizontalLine('-'));
            ConsoleOutput.PrintColorMessage("\nHighlighted Shipment Details:\n", ConsoleColor.Yellow);
            Console.WriteLine($"ID: {highlightedShipment.ShipmentId}");
            Console.WriteLine($"Type: {highlightedShipment.ShipmentType}");
            
            if (highlightedShipment.ShipmentType == database.ShipmentType.Inbound)
            {
                Console.WriteLine($"Shipper: {highlightedShipment.Shipper?.Name ?? "Unknown"}");
            }
            else
            {
                Console.WriteLine($"Shop: {highlightedShipment.Shop?.ShopName ?? "Unknown"}");
            }
            
            // Display shipment items
            Console.WriteLine("\nItems in shipment:");
            
            if (highlightedShipment.ShipmentItems != null && highlightedShipment.ShipmentItems.Any())
            {
                string[] headers = { "Product", "Category", "Quantity", "Unit" };
                List<string[]> rows = new List<string[]>();
                
                foreach (var item in highlightedShipment.ShipmentItems)
                {
                    rows.Add(new string[] { 
                        item.Product.Name, 
                        item.Product.Category,
                        item.Quantity.ToString(), 
                        item.Product.Unit 
                    });
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
            Console.WriteLine(ConsoleOutput.CenteredText("No unassigned shipments available", true));
        }
        
        Console.WriteLine(ConsoleOutput.HorizontalLine('='));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
