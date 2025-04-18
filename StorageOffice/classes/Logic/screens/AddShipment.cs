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
            if (shippers == null || !shippers.Any())
            {
                ConsoleOutput.PrintColorMessage("No shippers available. Please add a shipper first.\n", ConsoleColor.Yellow);
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

            int shipperIndex = ConsoleInput.GetUserInt("Select shipper ID: ");
            var selectedShipper = shippers.FirstOrDefault(s => s.ShipperId == shipperIndex);
            
            if (selectedShipper == null)
            {
                ConsoleOutput.PrintColorMessage("Invalid shipper ID.\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
                return;
            }

            // Create the shipment
            MenuHandler.db?.AddInboundShipment(selectedShipper.ShipperId);
            
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

            int shopId = ConsoleInput.GetUserInt("Select shop ID: ");
            var selectedShop = shops.FirstOrDefault(s => s.ShopId == shopId);
            
            if (selectedShop == null)
            {
                ConsoleOutput.PrintColorMessage("Invalid shop ID.\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to try again...");
                ConsoleInput.WaitForAnyKey();
                return;
            }

            // Create the shipment
            MenuHandler.db?.AddOutboundShipment(selectedShop.ShopId);
            
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
