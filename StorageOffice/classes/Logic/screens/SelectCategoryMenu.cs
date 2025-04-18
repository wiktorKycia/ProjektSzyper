using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

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

    private void ShowProductsInCategory(string category)
    {
        var categoryProductsMenu = new ShowCategoryProductsMenu(_user, category, () => _onExit.Invoke());
    }

    private void Display()
    {
        Console.Clear();
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
