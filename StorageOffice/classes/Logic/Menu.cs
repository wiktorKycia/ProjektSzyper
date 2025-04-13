using System;
using StorageOffice.classes.CLI;

namespace StorageOffice.classes.Logic
{
    // Delegate for menu actions
    public delegate void MenuAction();

    // Menu item representation
    public class MenuItem
    {
        public string Text { get; }
        public MenuAction Action { get; }
        public bool IsEnabled { get; set; } = true;

        public MenuItem(string text, MenuAction action)
        {
            Text = text;
            Action = action;
        }
    }

    // Menu system using delegates
    public class Menu
    {
        public string Title { get; }
        public string Description { get; }
        private readonly List<MenuItem> _menuItems = new();
        private readonly MenuAction _exitAction;
        
        public Menu(string title, string description, MenuAction exitAction = null)
        {
            Title = title;
            Description = description;
            _exitAction = exitAction ?? (() => { });
        }

        public Menu AddItem(string text, MenuAction action)
        {
            _menuItems.Add(new MenuItem(text, action));
            return this; // For fluent interface
        }

        public void Display()
        {
            // Convert menu items to radio options for display
            var options = _menuItems
                .Select(item => new RadioOption(item.Text) { IsEnabled = item.IsEnabled })
                .ToList();
            
            var menu = new MainMenu(Title, Description, new RadioSelect(options));
            
            // Configure keyboard actions
            menu.KeyboardActions[ConsoleKey.Enter] = () => 
            {
                var selectedIndex = options.FindIndex(o => o.IsSelected);
                if (selectedIndex >= 0 && selectedIndex < _menuItems.Count)
                {
                    var item = _menuItems[selectedIndex];
                    if (item.IsEnabled)
                    {
                        item.Action.Invoke();
                    }
                }
            };

            menu.KeyboardActions[ConsoleKey.Escape] = () => _exitAction.Invoke();
            
            // Run menu
            menu.Run();
        }
    }
    
    // Menu builder for more complex menu scenarios
    public class MenuBuilder
    {
        private readonly Dictionary<string, Menu> _menus = new();
        
        public MenuBuilder AddMenu(string id, string title, string description, MenuAction exitAction = null)
        {
            _menus[id] = new Menu(title, description, exitAction);
            return this;
        }
        
        public MenuBuilder AddMenuItem(string menuId, string text, MenuAction action)
        {
            if (_menus.TryGetValue(menuId, out var menu))
            {
                menu.AddItem(text, action);
            }
            return this;
        }
        
        public MenuBuilder AddSubMenu(string parentId, string text, string subMenuId)
        {
            if (_menus.TryGetValue(parentId, out var parentMenu) && 
                _menus.TryGetValue(subMenuId, out var subMenu))
            {
                parentMenu.AddItem(text, () => subMenu.Display());
            }
            return this;
        }
        
        public Menu GetMenu(string id)
        {
            return _menus.TryGetValue(id, out var menu) ? menu : null;
        }
    }
}