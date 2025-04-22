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
                    var productManager = new ShipmentProductManager(
                        MenuHandler.db?.GetShipmentById(_shipmentId),
                        () => { running = false; _onExit.Invoke(); },
                        _user
                    );
                }
                else
                {
                    running = false;
                    _onExit.Invoke();
                }
            }
        }
    }

    private void CreateInboundShipment(ref bool running)
    {
        try
        {
            // Get all shippers
            var shippers = MenuHandler.db?.GetAllShippers();
            if (shippers == null)
            {
                ConsoleOutput.PrintColorMessage("Unable to access shipper database.\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                running = false;
                return;
            }

            // Display shippers as a table
            string[] headers = { "ID", "Name", "Contact Info" };
            List<string[]> shipperRows = new List<string[]>();
            
            foreach (var shipper in shippers)
            {
                shipperRows.Add(new string[] { 
                    shipper.ShipperId.ToString(), 
                    shipper.Name, 
                    shipper.ContactInfo 
                });
            }
            
            Console.WriteLine("\nAvailable Shippers:");
            Console.WriteLine(ConsoleOutput.WriteTable(shipperRows, headers));

            // Offer option to select existing or create new shipper
            Console.WriteLine("\n1. Select existing shipper by ID");
            Console.WriteLine("2. Add a new shipper");
            int choice = ConsoleInput.GetUserInt("Choose an option: ");
            
            database.Shipper selectedShipper;
            
            if (choice == 2)
            {
                selectedShipper = HandleManualShipperEntry();
            }
            else
            {
                // Select existing shipper by ID
                int shipperIndex = ConsoleInput.GetUserInt("Select shipper ID: ");
                selectedShipper = shippers.FirstOrDefault(s => s.ShipperId == shipperIndex);
                
                if (selectedShipper == null)
                {
                    ConsoleOutput.PrintColorMessage("Invalid shipper ID.\n", ConsoleColor.Red);
                    Console.WriteLine("Press any key to try again...");
                    ConsoleInput.WaitForAnyKey();
                    return;
                }
            }

            // Create the shipment
            MenuHandler.db?.AddInboundShipment(selectedShipper.ShipperId);
            
            // Get the newly created shipment ID
            var shipments = MenuHandler.db?.GetAllNotCompletedInboundShipments();
            _shipmentId = shipments?.LastOrDefault()?.ShipmentId ?? -1;
            
            if (_shipmentId > 0)
            {                
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
    
    private database.Shipper HandleManualShipperEntry()
    {
        Console.Clear();
        Console.WriteLine(ConsoleOutput.Header("Add New Shipper"));
        
        // Get shipper name
        string shipperName = ConsoleInput.GetUserString("Enter shipper name: ");
        
        // Check if the shipper already exists
        var existingShipper = MenuHandler.db?.GetAllShippers()
            .FirstOrDefault(s => s.Name.Equals(shipperName, StringComparison.OrdinalIgnoreCase));
        
        if (existingShipper != null)
        {
            ConsoleOutput.PrintColorMessage($"Found existing shipper: {existingShipper.Name} (ID: {existingShipper.ShipperId})\n", ConsoleColor.Green);
            Console.WriteLine("Press any key to continue with this shipper...");
            ConsoleInput.WaitForAnyKey();
            return existingShipper;
        }
        
        // If shipper doesn't exist, collect details to create a new one
        string contactInfo = ConsoleInput.GetUserString("Enter contact information: ");
        
        // Confirm creation
        Console.WriteLine("\nShipper details:");
        Console.WriteLine($"Name: {shipperName}");
        Console.WriteLine($"Contact Info: {contactInfo}");
        
        Console.WriteLine("\nDo you want to create this shipper? (y/n)");
        var key = ConsoleInput.GetConsoleKey();
        
        if (key != ConsoleKey.Y)
        {
            ConsoleOutput.PrintColorMessage("Shipper creation cancelled.\n", ConsoleColor.Yellow);
            Console.WriteLine("Press any key to try again...");
            ConsoleInput.WaitForAnyKey();
            return HandleManualShipperEntry(); // Recursively try again
        }
        
        try
        {
            // Create the new shipper
            MenuHandler.db?.AddShipper(shipperName, contactInfo);
            
            // Get the newly created shipper
            var newShipper = MenuHandler.db?.GetAllShippers()
                .FirstOrDefault(s => s.Name.Equals(shipperName, StringComparison.OrdinalIgnoreCase));
            
            if (newShipper != null)
            {
                ConsoleOutput.PrintColorMessage($"Shipper '{shipperName}' created successfully with ID: {newShipper.ShipperId}\n", ConsoleColor.Green);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                return newShipper;
            }
            else
            {
                throw new InvalidOperationException("Failed to retrieve the newly created shipper.");
            }
        }
        catch (Exception ex)
        {
            ConsoleOutput.PrintColorMessage($"Error creating shipper: {ex.Message}\n", ConsoleColor.Red);
            Console.WriteLine("Press any key to try again...");
            ConsoleInput.WaitForAnyKey();
            return HandleManualShipperEntry(); // Recursively try again
        }
    }

    private void CreateOutboundShipment(ref bool running)
    {
        try
        {
            // Get all shops
            var shops = MenuHandler.db?.GetAllShops();
            if (shops == null)
            {
                ConsoleOutput.PrintColorMessage("Unable to access shop database.\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                running = false;
                return;
            }

            // Display shops as a table
            string[] headers = { "ID", "Name", "Location" };
            List<string[]> shopRows = new List<string[]>();
            
            foreach (var shop in shops)
            {
                shopRows.Add(new string[] { 
                    shop.ShopId.ToString(), 
                    shop.ShopName, 
                    shop.Location 
                });
            }
            
            Console.WriteLine("\nAvailable Shops:");
            Console.WriteLine(ConsoleOutput.WriteTable(shopRows, headers));

            // Offer option to select existing or create new shop
            Console.WriteLine("\n1. Select existing shop by ID");
            Console.WriteLine("2. Add a new shop");
            int choice = ConsoleInput.GetUserInt("Choose an option: ");
            
            database.Shop selectedShop;
            
            if (choice == 2)
            {
                selectedShop = HandleManualShopEntry();
            }
            else
            {
                // Select existing shop by ID
                int shopId = ConsoleInput.GetUserInt("Select shop ID: ");
                selectedShop = shops.FirstOrDefault(s => s.ShopId == shopId);
                
                if (selectedShop == null)
                {
                    ConsoleOutput.PrintColorMessage("Invalid shop ID.\n", ConsoleColor.Red);
                    Console.WriteLine("Press any key to try again...");
                    ConsoleInput.WaitForAnyKey();
                    return;
                }
            }

            // Create the shipment
            MenuHandler.db?.AddOutboundShipment(selectedShop.ShopId);
            
            // Get the newly created shipment ID
            var shipments = MenuHandler.db?.GetAllNotCompletedOutboundShipments();
            _shipmentId = shipments?.LastOrDefault()?.ShipmentId ?? -1;
            
            if (_shipmentId > 0)
            {                
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
    
    private database.Shop HandleManualShopEntry()
    {
        Console.Clear();
        Console.WriteLine(ConsoleOutput.Header("Add New Shop"));
        
        // Get shop name
        string shopName = ConsoleInput.GetUserString("Enter shop name: ");
        
        // Check if the shop already exists
        var existingShop = MenuHandler.db?.GetAllShops()
            .FirstOrDefault(s => s.ShopName.Equals(shopName, StringComparison.OrdinalIgnoreCase));
        
        if (existingShop != null)
        {
            ConsoleOutput.PrintColorMessage($"Found existing shop: {existingShop.ShopName} (ID: {existingShop.ShopId})\n", ConsoleColor.Green);
            Console.WriteLine("Press any key to continue with this shop...");
            ConsoleInput.WaitForAnyKey();
            return existingShop;
        }
        
        // If shop doesn't exist, collect details to create a new one
        string location = ConsoleInput.GetUserString("Enter shop location: ");
        
        // Confirm creation
        Console.WriteLine("\nShop details:");
        Console.WriteLine($"Name: {shopName}");
        Console.WriteLine($"Location: {location}");
        
        Console.WriteLine("\nDo you want to create this shop? (y/n)");
        var key = ConsoleInput.GetConsoleKey();
        
        if (key != ConsoleKey.Y)
        {
            ConsoleOutput.PrintColorMessage("Shop creation cancelled.\n", ConsoleColor.Yellow);
            Console.WriteLine("Press any key to try again...");
            ConsoleInput.WaitForAnyKey();
            return HandleManualShopEntry(); // Recursively try again
        }
        
        try
        {
            // Create the new shop
            MenuHandler.db?.AddShop(shopName, location);
            
            // Get the newly created shop
            var newShop = MenuHandler.db?.GetAllShops()
                .FirstOrDefault(s => s.ShopName.Equals(shopName, StringComparison.OrdinalIgnoreCase));
            
            if (newShop != null)
            {
                ConsoleOutput.PrintColorMessage($"Shop '{shopName}' created successfully with ID: {newShop.ShopId}\n", ConsoleColor.Green);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                return newShop;
            }
            else
            {
                throw new InvalidOperationException("Failed to retrieve the newly created shop.");
            }
        }
        catch (Exception ex)
        {
            ConsoleOutput.PrintColorMessage($"Error creating shop: {ex.Message}\n", ConsoleColor.Red);
            Console.WriteLine("Press any key to try again...");
            ConsoleInput.WaitForAnyKey();
            return HandleManualShopEntry(); // Recursively try again
        }
    }

    private void Display()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        string content = _heading;
        
        Console.WriteLine(ConsoleOutput.UIFrame(_title, content));

        foreach (var action in _displayKeyboardActions)
        {
            Console.WriteLine($"{action.Key} - {action.Value}");
        }

        Console.WriteLine(ConsoleOutput.HorizontalLine('-'));
    }
}
