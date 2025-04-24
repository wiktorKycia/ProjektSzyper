using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Displays a menu showing all products currently in stock.
/// Allows the user to view product details and navigate back to the previous menu.
/// </summary>
public class ShowAllProductsMenu
{
    private readonly string _title;
    private readonly User _user;
    private readonly Action _onExit;
    private readonly List<database.Product> _products;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAllProductsMenu"/> class.
    /// </summary>
    /// <param name="user">The user viewing the products.</param>
    /// <param name="onExit">The action to execute when exiting the menu.</param>
    public ShowAllProductsMenu(User user, Action onExit)
    {
        _title = "All Products in Stock";
        _user = user;
        _onExit = onExit;
        _products = MenuHandler.db?.GetAllProducts() ?? new List<database.Product>();

        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>()
        {
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };

        _displayKeyboardActions = new Dictionary<string, string>()
        {
            { "<Esc>", "back" }
        };

        Run();
    }

    /// <summary>
    /// Runs the main loop for the menu, handling user input and navigation.
    /// </summary>
    private void Run()
    {
        Display();

        while (true)
        {
            var key = ConsoleInput.GetConsoleKey();
            if (key == ConsoleKey.Escape)
            {
                _onExit.Invoke();
                break;
            }
        }
    }

    /// <summary>
    /// Displays the list of all products currently in stock.
    /// </summary>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = "Current inventory of all products:\n\n";

        if (_products != null && _products.Any())
        {
            string[] headers = { "ID", "Product", "Category", "Stock", "Unit", "Last Updated" };
            List<string[]> productRows = new List<string[]>();

            foreach (var product in _products)
            {
                productRows.Add(new string[] {
                    product.ProductId.ToString(),
                    product.Name,
                    product.Category,
                    product.Stock?.Quantity.ToString() ?? "0",
                    product.Unit,
                    product.Stock?.LastUpdated.ToString("dd/MM/yyyy") ?? "N/A"
                });
            }

            content += ConsoleOutput.WriteTable(productRows, headers);
        }
        else
        {
            content += "No products found in stock.";
        }

        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
