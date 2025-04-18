using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

public class ShipmentProductManager
{
    private database.Shipment _shipment;
    private readonly Action _onExit;
    private readonly User _user;
    private readonly string _title;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    public ShipmentProductManager(database.Shipment shipment, Action onExit, User user)
    {
        _shipment = shipment;
        _onExit = onExit;
        _user = user;
        _title = $"Manage Products for Shipment #{shipment.ShipmentId}";

        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>()
        {
            { ConsoleKey.Escape, () => _onExit.Invoke() }
        };

        _displayKeyboardActions = new Dictionary<string, string>()
        {
            { "<Esc>", "back" }
        };

        Run();
    }

    private void Run()
    {
        bool adding = true;
        while (adding)
        {
            Display();
            DisplayCurrentShipmentItems();
            
            Console.WriteLine("\n1. Add product");
            Console.WriteLine("2. Finish adding products");
            if (_shipment.ShipmentItems != null && _shipment.ShipmentItems.Any())
            {
                Console.WriteLine("3. Mark shipment as complete");
            }
            
            int choice = ConsoleInput.GetUserInt("Select an option: ");
            
            switch (choice)
            {
                case 1:
                    AddProductToShipment();
                    break;
                case 2:
                    adding = false;
                    _onExit.Invoke();
                    break;
                case 3:
                    if (_shipment.ShipmentItems != null && _shipment.ShipmentItems.Any())
                    {
                        if (ConfirmCompletion())
                        {
                            try
                            {
                                MenuHandler.db?.MarkShipmentAsDone(_shipment.ShipmentId);
                                ConsoleOutput.PrintColorMessage("Shipment marked as completed successfully!\n", ConsoleColor.Green);
                                Console.WriteLine("Press any key to continue...");
                                ConsoleInput.WaitForAnyKey();
                                adding = false;
                                _onExit.Invoke();
                            }
                            catch (Exception ex)
                            {
                                ConsoleOutput.PrintColorMessage($"Error: {ex.Message}\n", ConsoleColor.Red);
                                Console.WriteLine("Press any key to continue...");
                                ConsoleInput.WaitForAnyKey();
                            }
                        }
                    }
                    break;
                default:
                    ConsoleOutput.PrintColorMessage("Invalid option. Please try again.\n", ConsoleColor.Yellow);
                    Console.WriteLine("Press any key to continue...");
                    ConsoleInput.WaitForAnyKey();
                    break;
            }
        }
    }

    private void Display()
    {
        Console.Clear();
        string content = $"Adding Products to {(_shipment.ShipmentType == database.ShipmentType.Inbound ? "Inbound" : "Outbound")} Shipment #{_shipment.ShipmentId}";
        
        if (_shipment.ShipmentType == database.ShipmentType.Inbound)
        {
            content += $"\nShipper: {_shipment.Shipper?.Name ?? "Not assigned"}";
        }
        else
        {
            content += $"\nShop: {_shipment.Shop?.ShopName ?? "Not assigned"}";
        }
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }
        
        Console.WriteLine(ConsoleOutput.HorizontalLine('-'));
    }
    
    private void DisplayCurrentShipmentItems()
    {
        try
        {
            Console.WriteLine("\nCurrent Shipment Items:");
            
            if (_shipment?.ShipmentItems == null || !_shipment.ShipmentItems.Any())
            {
                Console.WriteLine("No items added to this shipment yet.");
                return;
            }
            
            string[] headers = { "Product", "Category", "Quantity", "Unit" };
            List<string[]> itemRows = new List<string[]>();
            
            foreach (var item in _shipment.ShipmentItems)
            {
                itemRows.Add(new string[] { 
                    item.Product.Name, 
                    item.Product.Category, 
                    item.Quantity.ToString(), 
                    item.Product.Unit 
                });
            }
            
            Console.WriteLine(ConsoleOutput.WriteTable(itemRows, headers));
        }
        catch (Exception ex)
        {
            ConsoleOutput.PrintColorMessage($"Error displaying shipment items: {ex.Message}\n", ConsoleColor.Red);
        }
    }
    
    private void AddProductToShipment()
    {
        try
        {
            // Get all products
            var products = MenuHandler.db?.GetAllProducts();
            if (products == null || !products.Any())
            {
                ConsoleOutput.PrintColorMessage("No products available.\n", ConsoleColor.Yellow);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                return;
            }

            // Display products in a table
            string[] headers = { "ID", "Product", "Category", "Stock", "Unit" };
            List<string[]> productRows = new List<string[]>();
            
            foreach (var product in products)
            {
                productRows.Add(new string[] { 
                    product.ProductId.ToString(), 
                    product.Name, 
                    product.Category, 
                    product.Stock.Quantity.ToString(), 
                    product.Unit 
                });
            }
            
            Console.WriteLine("\nAvailable Products:");
            Console.WriteLine(ConsoleOutput.WriteTable(productRows, headers));

            int productId = ConsoleInput.GetUserInt("Select product ID: ");
            var selectedProduct = products.FirstOrDefault(p => p.ProductId == productId);
            
            if (selectedProduct == null)
            {
                ConsoleOutput.PrintColorMessage("Invalid product ID.\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
                return;
            }
            
            // Get quantity
            int maxQuantity = _shipment.ShipmentType == database.ShipmentType.Outbound ? 
                selectedProduct.Stock.Quantity : 10000;
                
            if (maxQuantity <= 0 && _shipment.ShipmentType == database.ShipmentType.Outbound)
            {
                ConsoleOutput.PrintColorMessage("This product is out of stock.\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                return;
            }
            
            int quantity = ConsoleInput.GetUserInt($"Enter quantity (1-{maxQuantity}): ");
            
            if (quantity <= 0 || quantity > maxQuantity)
            {
                ConsoleOutput.PrintColorMessage($"Quantity must be between 1 and {maxQuantity}.\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
                return;
            }
            
            // Add to shipment
            MenuHandler.db?.AddShipmentItem(quantity, _shipment.ShipmentId, selectedProduct.ProductId);
            
            // Refresh shipment data
            _shipment = MenuHandler.db.GetShipmentById(_shipment.ShipmentId);
            
            ConsoleOutput.PrintColorMessage("Product added to shipment successfully!\n", ConsoleColor.Green);
            Console.WriteLine("Press any key to continue...");
            ConsoleInput.WaitForAnyKey();
        }
        catch (Exception ex)
        {
            ConsoleOutput.PrintColorMessage($"Error: {ex.Message}\n", ConsoleColor.Red);
            Console.WriteLine("Press any key to continue...");
            ConsoleInput.WaitForAnyKey();
        }
    }
    
    private bool ConfirmCompletion()
    {
        Console.WriteLine("\nDo you want to mark this shipment as completed? (y/n): ");
        var key = ConsoleInput.GetConsoleKey();
        return key == ConsoleKey.Y;
    }
}
