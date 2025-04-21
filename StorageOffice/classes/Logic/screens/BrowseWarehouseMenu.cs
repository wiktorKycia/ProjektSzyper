using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

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

    private void ShowAllProducts()
    {
        var allProductsMenu = new ShowAllProductsMenu(_user, () => _onExit.Invoke());
    }

    private void ShowProductsByCategory()
    {
        var categoriesMenu = new SelectCategoryMenu(_user, () => _onExit.Invoke());
    }

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
