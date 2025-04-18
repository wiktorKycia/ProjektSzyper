using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.UsersManagement.Modules;

namespace StorageOffice.classes.Logic;

public class AddShipment
{
    private readonly string _title;
    private readonly string _heading;
    private readonly Action _onExit;
    private readonly database.ShipmentType _shipmentType;
    private readonly User _user;
    private int _shipmentId;
    private readonly Dictionary<ConsoleKey, KeyboardAction> _keyboardActions;
    private readonly Dictionary<string, string> _displayKeyboardActions;

    internal AddShipment(Action onExit, User user, database.ShipmentType shipmentType)
    {
        _title = shipmentType == database.ShipmentType.Inbound ? "Add Inbound Shipment" : "Add Outbound Shipment";
        _heading = shipmentType == database.ShipmentType.Inbound ? "Add Inbound Shipment (Import)" : "Add Outbound Shipment (Export)";
        _onExit = onExit;
        _shipmentType = shipmentType;
        _user = user;
        _shipmentId = -1;

        _keyboardActions = new Dictionary<ConsoleKey, KeyboardAction>(){
            { ConsoleKey.Escape, () => onExit.Invoke() }
        };
        _displayKeyboardActions = new Dictionary<string, string>(){
            { "<Esc>", "cancel" },
            { "Any other key", "continue" }
        };

        Run();
    }

    private void Run()
    {
        bool running = true;
        while (running)
        {
            Display();
            var key = ConsoleInput.GetConsoleKey();
            if (_keyboardActions.ContainsKey(key))
            {
                _keyboardActions[key]();
                running = false;
            }
            else
            {
                if (_shipmentType == database.ShipmentType.Inbound)
                {
                    CreateInboundShipment(ref running);
                }
                else
                {
                    CreateOutboundShipment(ref running);
                }

                // After creating the shipment, add products to it if valid shipmentId
                if (_shipmentId > 0)
                {
                    AddProductsToShipment(_shipmentId);
                    
                    // Ask if the user wants to mark the shipment as completed
                    if (ConfirmCompletion())
                    {
                        try
                        {
                            MenuHandler.db?.MarkShipmentAsDone(_shipmentId);
                            ConsoleOutput.PrintColorMessage("Shipment marked as completed successfully!\n", ConsoleColor.Green);
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
                }

                running = false;
                _onExit.Invoke();
            }
        }
    }

    private void CreateInboundShipment(ref bool running)
    {
        try
        {
            // Get all shippers
            var shippers = MenuHandler.db?.GetAllShippers();
            if (shippers == null || !shippers.Any())
            {
                ConsoleOutput.PrintColorMessage("No shippers available. Please add a shipper first.\n", ConsoleColor.Yellow);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                running = false;
                return;
            }

            // Display shippers for selection
            Console.WriteLine("\nAvailable Shippers:");
            for (int i = 0; i < shippers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {shippers[i].Name} - {shippers[i].ContactInfo}");
            }

            int shipperIndex = ConsoleInput.GetUserInt("Select shipper (number): ") - 1;
            int shipperId = shippers[shipperIndex].ShipperId;

            // Create the shipment
            MenuHandler.db?.AddInboundShipment(shipperId);
            
            // Get the newly created shipment ID
            var shipments = MenuHandler.db?.GetAllNotCompletedInboundShipments();
            _shipmentId = shipments?.LastOrDefault()?.ShipmentId ?? -1;
            
            // Assign the logged-in user to the shipment
            if (_shipmentId > 0)
            {
                try
                {
                    int currentUserId = MenuHandler.db?.GetUserIdByUsername(_user.Username) ?? 0;
                    if (currentUserId > 0)
                    {
                        MenuHandler.db?.UpdateUserAssaignedToShipment(currentUserId, _shipmentId);
                    }
                }
                catch (Exception)
                {
                    // If we can't assign the user, continue anyway
                }
                
                ConsoleOutput.PrintColorMessage("Inbound shipment created successfully!\n", ConsoleColor.Green);
                Console.WriteLine("Press any key to add products to this shipment...");
                ConsoleInput.WaitForAnyKey();
            }
            else
            {
                ConsoleOutput.PrintColorMessage("Failed to create shipment.\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                running = false;
            }
        }
        catch (Exception ex)
        {
            ConsoleOutput.PrintColorMessage($"Error: {ex.Message}\n", ConsoleColor.Red);
            Console.WriteLine("Press any key to try again...");
            ConsoleInput.WaitForAnyKey();
        }
    }

    private void CreateOutboundShipment(ref bool running)
    {
        try
        {
            // Get all shops
            var shops = MenuHandler.db?.GetAllShops();
            if (shops == null || !shops.Any())
            {
                ConsoleOutput.PrintColorMessage("No shops available. Please add a shop first.\n", ConsoleColor.Yellow);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                running = false;
                return;
            }

            // Display shops for selection
            Console.WriteLine("\nAvailable Shops:");
            for (int i = 0; i < shops.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {shops[i].ShopName} - {shops[i].Location}");
            }

            int shopIndex = ConsoleInput.GetUserInt("Select shop (number): ") - 1;
            int shopId = shops[shopIndex].ShopId;

            // Create the shipment
            MenuHandler.db?.AddOutboundShipment(shopId);
            
            // Get the newly created shipment ID
            var shipments = MenuHandler.db?.GetAllNotCompletedOutboundShipments();
            _shipmentId = shipments?.LastOrDefault()?.ShipmentId ?? -1;
            
            // Assign the logged-in user to the shipment
            if (_shipmentId > 0)
            {
                try
                {
                    int currentUserId = MenuHandler.db?.GetUserIdByUsername(_user.Username) ?? 0;
                    if (currentUserId > 0)
                    {
                        MenuHandler.db?.UpdateUserAssaignedToShipment(currentUserId, _shipmentId);
                    }
                }
                catch (Exception)
                {
                    // If we can't assign the user, continue anyway
                }
                
                ConsoleOutput.PrintColorMessage("Outbound shipment created successfully!\n", ConsoleColor.Green);
                Console.WriteLine("Press any key to add products to this shipment...");
                ConsoleInput.WaitForAnyKey();
            }
            else
            {
                ConsoleOutput.PrintColorMessage("Failed to create shipment.\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                running = false;
            }
        }
        catch (Exception ex)
        {
            ConsoleOutput.PrintColorMessage($"Error: {ex.Message}\n", ConsoleColor.Red);
            Console.WriteLine("Press any key to try again...");
            ConsoleInput.WaitForAnyKey();
        }
    }

    private void AddProductsToShipment(int shipmentId)
    {
        bool addingProducts = true;
        
        while (addingProducts)
        {
            Console.Clear();
            Console.WriteLine($"{_title} - Adding Products");
            Console.WriteLine(ConsoleOutput.HorizontalLine('-'));
            
            // Show current shipment items
            DisplayCurrentShipmentItems(shipmentId);
            
            Console.WriteLine("\n1. Add product to shipment");
            Console.WriteLine("2. Finish adding products");
            
            int choice = ConsoleInput.GetUserInt("Select an option: ");
            
            if (choice == 1)
            {
                AddProductToShipment(shipmentId);
            }
            else
            {
                addingProducts = false;
            }
        }
    }
    
    private void AddProductToShipment(int shipmentId)
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

            // Display products for selection
            Console.WriteLine("\nAvailable Products:");
            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {products[i].Name} - {products[i].Category} - Current Stock: {products[i].Stock.Quantity} {products[i].Unit}");
            }

            int productIndex = ConsoleInput.GetUserInt("Select product (number): ") - 1;
            int productId = products[productIndex].ProductId;
            
            // Get quantity
            int maxQuantity = _shipmentType == database.ShipmentType.Outbound ? products[productIndex].Stock.Quantity : 10000;
            int quantity = ConsoleInput.GetUserInt($"Enter quantity (1-{maxQuantity}): ");
            
            // Add to shipment
            MenuHandler.db?.AddShipmentItem(quantity, shipmentId, productId);
            
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
    
    private void DisplayCurrentShipmentItems(int shipmentId)
    {
        try
        {
            Console.WriteLine("\nCurrent Shipment Items:");
            Console.WriteLine("--------------------------------------------------");
            
            var shipment = MenuHandler.db?.GetShipmentById(shipmentId);
            
            if (shipment?.ShipmentItems == null || !shipment.ShipmentItems.Any())
            {
                Console.WriteLine("No items added to this shipment yet.");
                return;
            }
            
            Console.WriteLine($"{"Product",-20} {"Category",-15} {"Quantity",-10} {"Unit",-10}");
            Console.WriteLine("--------------------------------------------------");
            
            foreach (var item in shipment.ShipmentItems)
            {
                Console.WriteLine($"{item.Product.Name,-20} {item.Product.Category,-15} {item.Quantity,-10} {item.Product.Unit,-10}");
            }
        }
        catch (Exception ex)
        {
            ConsoleOutput.PrintColorMessage($"Error displaying shipment items: {ex.Message}\n", ConsoleColor.Red);
        }
    }
    
    private bool ConfirmCompletion()
    {
        Console.WriteLine("\nDo you want to mark this shipment as completed? (y/n): ");
        var key = ConsoleInput.GetConsoleKey();
        return key == ConsoleKey.Y;
    }

    private void Display()
    {
        Console.Clear();
        string content = _heading;
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }

        Console.WriteLine(ConsoleOutput.HorizontalLine('-'));
    }
}
