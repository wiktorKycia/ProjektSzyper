using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Models;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Represents the menu for browsing the warehouse.
/// This class provides an interactive console-based workflow for viewing all products
/// or filtering products by category.
/// </summary>
/// <remarks>
/// The menu allows users to navigate through options using keyboard inputs and
/// invokes the appropriate functionality based on the selected option.
/// </remarks>
/// <param name="user">
/// The user interacting with the menu.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user exits the menu.
/// </param>

public class BrowseWarehouseMenu
{
    private readonly string _title;
    private readonly User _user;
    private readonly Action _onExit;
    private readonly RadioSelect _select;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public BrowseWarehouseMenu(User user, Action onExit)
    {
        _title = "Browse Warehouse";
        _user = user;
        _onExit = onExit;
        
        var options = new List<RadioOption>
        {
            new RadioOption("Show All Products", () => ShowAllProducts()),
            new RadioOption("Show Products by Category", () => ShowProductsByCategory())
        };
        
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

    /// <summary>
    /// Executes the main workflow for the warehouse browsing menu.
    /// Displays the user interface, handles user input for navigation and selection,
    /// and invokes the selected operation.
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
                else if (key == ConsoleKey.Enter)
                {
                    _keyboardActions[key]();
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
    /// Opens the menu to display all products in the warehouse.
    /// </summary>
    /// <remarks>
    /// This method initializes the `ShowAllProductsMenu` and passes control to it.
    /// </remarks>

    private void ShowAllProducts()
    {
        var allProductsMenu = new ShowAllProductsMenu(_user, () => _onExit.Invoke());
    }

    /// <summary>
    /// Opens the menu to display products filtered by category.
    /// </summary>
    /// <remarks>
    /// This method initializes the `SelectCategoryMenu` and passes control to it.
    /// </remarks>

    private void ShowProductsByCategory()
    {
        var categoriesMenu = new SelectCategoryMenu(_user, () => _onExit.Invoke());
    }

    /// <summary>
    /// Displays the user interface for the warehouse browsing menu.
    /// Shows the available options and provides navigation instructions for the user.
    /// </summary>
    /// <remarks>
    /// The method dynamically builds the content to display the menu options and
    /// ensures proper formatting for the console output.
    /// </remarks>

    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = "Select an option to browse the warehouse:\n\n";
        
        if (_select.Options != null)
        {
            foreach (var option in _select.Options)
            {
                content += ConsoleOutput.CenteredText(option.ToString() + "\n", true);
            }
        }
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
