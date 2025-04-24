using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Models;

namespace StorageOffice.classes.Logic;


/// <summary>
/// Represents the menu for selecting a product category.
/// This class provides an interactive console-based workflow for viewing and selecting
/// product categories.
/// </summary>
/// <remarks>
/// The menu allows navigation through a list of product categories and invokes the appropriate
/// operation based on the user's selection.
/// </remarks>
/// <param name="user">
/// The user interacting with the menu.
/// </param>
/// <param name="onExit">
/// An action to be invoked when the user exits the menu.
/// </param>
public class SelectCategoryMenu
{
    private readonly string _title;
    private readonly User _user;
    private readonly Action _onExit;
    private readonly RadioSelect _select;
    private readonly List<string> _categories;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public SelectCategoryMenu(User user, Action onExit)
    {
        _title = "Select Product Category";
        _user = user;
        _onExit = onExit;
        
        // Get all products and extract unique categories
        var products = MenuHandler.db?.GetAllProducts() ?? new List<database.Product>();
        _categories = products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
        
        if (!_categories.Any())
        {
            var error = new Error("No product categories found.", () => onExit.Invoke());
            return;
        }
        
        var options = _categories.Select(c => new RadioOption(
            c,
            () => ShowProductsInCategory(c)
        )).ToList();
        
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
    /// Executes the main workflow for the category selection menu.
    /// Displays the user interface, handles user input for navigation and selection,
    /// and invokes the appropriate operation based on the selected category.
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
    /// Opens the menu to display products in the selected category.
    /// </summary>
    /// <param name="category">
    /// The name of the category whose products are to be displayed.
    /// </param>
    /// <remarks>
    /// This method initializes the `ShowCategoryProductsMenu` and passes control to it.
    /// </remarks>
    private void ShowProductsInCategory(string category)
    {
        var categoryProductsMenu = new ShowCategoryProductsMenu(_user, category, () => _onExit.Invoke());
    }

    /// <summary>
    /// Displays the user interface for the category selection menu.
    /// Shows the list of product categories and provides navigation instructions for the user.
    /// </summary>
    /// <remarks>
    /// The method dynamically builds the content to display the menu options and ensures
    /// proper formatting for the console output.
    /// </remarks>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = "Select a product category to view:\n\n";
        
        if (_select.Options != null)
        {
            foreach (var option in _select.Options)
            {
                content += ConsoleOutput.CenteredText(option.ToString() + "\n", true);
            }
        }
        else
        {
            content += "No categories available.";
        }
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
