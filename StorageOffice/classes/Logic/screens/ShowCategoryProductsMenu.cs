using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;


/// <summary>
/// Displays a menu showing all products in a specific category.
/// Allows the user to view product details and navigate back to the previous menu.
/// </summary>
public class ShowCategoryProductsMenu
{
    private readonly string _title;
    private readonly User _user;
    private readonly string _category;
    private readonly Action _onExit;
    private readonly List<database.Product> _products;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCategoryProductsMenu"/> class.
    /// </summary>
    /// <param name="user">The user viewing the products.</param>
    /// <param name="category">The category of products to display.</param>
    /// <param name="onExit">The action to execute when exiting the menu.</param>
    public ShowCategoryProductsMenu(User user, string category, Action onExit)
    {
        _title = $"Products in Category: {category}";
        _user = user;
        _category = category;
        _onExit = onExit;

        // Get products in this category
        var allProducts = MenuHandler.db?.GetAllProducts() ?? new List<database.Product>();
        _products = allProducts.Where(p => p.Category == category).ToList();

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
    /// Displays the list of products in the selected category.
    /// </summary>
    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = $"Products in category '{_category}':\n\n";

        if (_products != null && _products.Any())
        {
            string[] headers = { "ID", "Product", "Stock", "Unit", "Last Updated", "Description" };
            List<string[]> productRows = new List<string[]>();

            foreach (var product in _products)
            {
                productRows.Add(new string[] {
                    product.ProductId.ToString(),
                    product.Name,
                    product.Stock?.Quantity.ToString() ?? "0",
                    product.Unit,
                    product.Stock?.LastUpdated.ToString("dd/MM/yyyy") ?? "N/A",
                    product.Description
                });
            }

            content += ConsoleOutput.WriteTable(productRows, headers);
        }
        else
        {
            content += "No products found in this category.";
        }

        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
    }
}
