using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

public class ShowAllProductsMenu
{
    private readonly string _title;
    private readonly User _user;
    private readonly Action _onExit;
    private readonly List<database.Product> _products;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

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

    private void Display()
    {
        Console.Clear();
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
