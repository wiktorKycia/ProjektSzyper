using System;
using Microsoft.EntityFrameworkCore.Migrations;
using StorageOffice.classes.CLI;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

public static class MenuHandler
{
    public static database.StorageDatabase? db = null;

    internal static void Start(User user)
    {
        if(PasswordManager.CheckFile())
        {
            var loginMenu = new Login(
                title: "Login menu",
                heading: "Welcome to the logistics warehouse management system!",
                nextMenu: () => {MainMenu(user);},
                user: user
            );
        }
        else
        {
            var firstUserMenu = new FirstUser(
                title: "Register first user",
                heading: "No user has been created on the system, so the details currently provided will be used to create the first administrator. Enter the details for him/her.\n",
                nextMenu: () => {MainMenu(user);},
                user: user
            );
        }
    }

    internal static void MainMenu(User user)
    {
        RBAC rbac = new();
        // prepare options according to permissions
        Dictionary<Permission, RadioOption> options = new Dictionary<Permission, RadioOption>
        {
            {Permission.BrowseWarehouse, new RadioOption("Warehouse", () => {DetailsMenu(user, "Warehouse");})},
            {Permission.AssignTask, new RadioOption("Tasks", () => {DetailsMenu(user, "Assign task");})},
            {Permission.DoTasks, new RadioOption("Tasks", () => {DetailsMenu(user, "Do tasks");})},
            {Permission.ManageShipments, new RadioOption("Shipments", () => {ManageShipmentsMenu(user);})},
            {Permission.BrowseShipments, new RadioOption("Shipments", () => {BrowseShipmentsMenu(user);})},
            {Permission.ManageUsers, new RadioOption("Manage users", () => {ManageUsers(user);})},
            {Permission.ViewLogs, new RadioOption("View logs", () => {Logs(user);})}
        };

        // check if user has permission to each option
        var radioOptions = options
            .Where(option => rbac.HasPermission(user, option.Key))
            .Select(option => option.Value)
            .ToList();


        var menu = new MainMenu("Storage System manager", $"Logged in as {user.Username}", new RadioSelect(radioOptions));
    }

    internal static void DetailsMenu(User user, string _details)
    {
        var details = new Details(_details, () => {MainMenu(user);});
    }

    internal static void Logs(User user)
    {
        try
        {
            var logs = new Logs(LogManager.GetAllLogs(), () => {MainMenu(user);});
        }
        catch(FileNotFoundException e)
        {
            var menu = new Error(
                text: e.Message,
                onExit: () => {MainMenu(user);}
            );
        }
    }

    internal static void ManageUsers(User user)
    {
        var manageUsers = new ManageUsers("Users", "", new RadioSelect(new List<RadioOption>
        {
            new RadioOption("Add user", () => {AddUser(user);}),
            new RadioOption("Edit user", () => {EditUserMenu(user);}),
            new RadioOption("Delete user", () => {DeleteUser(user);}),
            new RadioOption("View users", () => {ViewUsers(user);})
        }), () => {MainMenu(user);});
    }

    internal static void ViewUsers(User user)
    {
        var users = PasswordManager.GetAllUsers();
        var viewUsers = new Users(users, () => {ManageUsers(user);});
    }

    internal static void AddUser(User user)
    {
        var addUser = new AddUser(() => {ManageUsers(user);}, user);
    }

    internal static void DeleteUser(User user)
    {
        var users = PasswordManager.GetAllUsers();
        var checkBoxOptions = users.Select(u => new CheckBoxOption(u.Username, ()=>{
            PasswordManager.DeleteUser(u.Username);
            db!.DeleteUser(db.GetUserIdByUsername(u.Username));
        })).ToList();

        var deleteUser = new DeleteUser(new CheckBoxSelect(checkBoxOptions), () => {ManageUsers(user);});
    }

    internal static void EditUserMenu(User user)
    {
        var users = PasswordManager.GetAllUsers();
        var radioOptions = users.Select(u => new RadioOption(u.Username, () => {
            EditConcreteUser(user, u.Username);
        })).ToList();

        var editUser = new EditUser("Edit user", new RadioSelect(radioOptions), () => {ManageUsers(user);});
    }

    internal static void EditConcreteUser(User user, string username)
    {
        var radioOptions = new List<RadioOption>
        {
            new RadioOption("Edit Username", () => { EditUsername(user, username); }),
            new RadioOption("Edit Password", () => { EditPassword(user, username); }),
            new RadioOption("Edit Role", () => { EditRole(user, username); })
        };
        var editConcreteUser = new EditConcreteUser(username:username, select:new RadioSelect(radioOptions), onExit:() => {EditUserMenu(user);});
    }

    internal static void EditUsername(User user, string username)
    {
        var editUsername = new EditUsername(username, () => {ManageUsers(user);});
    }
    internal static void EditPassword(User user, string username)
    {
        var editPassword = new EditPassword(username, () => {ManageUsers(user);});
    }
    internal static void EditRole(User user, string username)
    {
        var editRole = new EditRole(username, () => {ManageUsers(user);});
    }

    internal static void ManageShipmentsMenu(User user)
    {
        var manageShipments = new ManageShipments(new RadioSelect(new List<RadioOption>
        {
            new RadioOption("Create Inbound Shipment (Import)", () => AddShipment(user, database.ShipmentType.Inbound)),
            new RadioOption("Create Outbound Shipment (Export)", () => AddShipment(user, database.ShipmentType.Outbound)),
            new RadioOption("View All Shipments", () => ViewAllShipments(user)),
            new RadioOption("View Pending Shipments", () => ViewPendingShipments(user)),
            new RadioOption("View Completed Shipments", () => ViewCompletedShipments(user)),
            new RadioOption("Manage My Assigned Shipments", () => ManageAssignedShipments(user))
        }), () => MainMenu(user));
    }

    internal static void BrowseShipmentsMenu(User user)
    {
        var browseShipments = new ManageShipments(new RadioSelect(new List<RadioOption>
        {
            new RadioOption("View All Shipments", () => ViewAllShipments(user)),
            new RadioOption("View Pending Shipments", () => ViewPendingShipments(user)),
            new RadioOption("View Completed Shipments", () => ViewCompletedShipments(user))
        }), () => MainMenu(user));
    }

    internal static void AddShipment(User user, database.ShipmentType shipmentType)
    {
        var addShipment = new AddShipment(() => ManageShipmentsMenu(user), user, shipmentType);
    }

    internal static void ViewAllShipments(User user)
    {
        var shipments = db?.GetAllShipments() ?? new List<database.Shipment>();
        var viewShipments = new ViewShipments(shipments, () => ManageShipmentsMenu(user), "All Shipments");
    }

    internal static void ViewPendingShipments(User user)
    {
        var shipments = db?.GetNotCompletedShipments() ?? new List<database.Shipment>();
        var viewShipments = new ViewShipments(shipments, () => ManageShipmentsMenu(user), "Pending Shipments", false, true);
    }

    internal static void ViewCompletedShipments(User user)
    {
        var allShipments = db?.GetAllShipments() ?? new List<database.Shipment>();
        var completedShipments = allShipments.Where(s => s.IsCompleted).ToList();
        var viewShipments = new ViewShipments(completedShipments, () => ManageShipmentsMenu(user), "Completed Shipments", true);
    }

    internal static void ManageAssignedShipments(User user)
    {
        try {
            int userId = db?.GetUserIdByUsername(user.Username) ?? 0;
            if (userId == 0)
            {
                throw new InvalidOperationException("User not found in database");
            }
            
            var shipments = db?.GetNotCompletedShipmentsAssignedToUser(userId) ?? new List<database.Shipment>();
            
            if (!shipments.Any())
            {
                var error = new Error("You don't have any pending shipments assigned to you.", () => ManageShipmentsMenu(user));
                return;
            }
            
            var shipmentOptions = shipments.Select(s => new RadioOption(
                $"ID: {s.ShipmentId} - {s.ShipmentType} - {(s.ShipmentType == database.ShipmentType.Inbound ? s.Shipper?.Name : s.Shop?.ShopName)}", 
                () => EditAssignedShipment(user, s)
            )).ToList();
            
            var selectShipment = new EditUser("Your Assigned Shipments", new RadioSelect(shipmentOptions), () => ManageShipmentsMenu(user));
        }
        catch (Exception ex)
        {
            var error = new Error($"Error: {ex.Message}", () => ManageShipmentsMenu(user));
        }
    }

    internal static void EditAssignedShipment(User user, database.Shipment shipment)
    {
        var shipmentOptions = new List<RadioOption>
        {
            new RadioOption("Add Products", () => AddProductsToShipment(user, shipment)),
            new RadioOption("Mark as Complete", () => MarkShipmentComplete(user, shipment))
        };
        
        var editShipment = new EditShipment(shipment, new RadioSelect(shipmentOptions), () => ManageAssignedShipments(user), user);
    }

    internal static void AddProductsToShipment(User user, database.Shipment shipment)
    {
        try
        {
            // Get all products
            var products = db?.GetAllProducts();
            if (products == null || !products.Any())
            {
                var error = new Error("No products available.", () => EditAssignedShipment(user, shipment));
                return;
            }

            bool adding = true;
            while (adding)
            {
                Console.Clear();
                Console.WriteLine($"Adding Products to Shipment {shipment.ShipmentId}");
                Console.WriteLine(ConsoleOutput.HorizontalLine('-'));
                
                // Display current shipment items
                Console.WriteLine("\nCurrent Shipment Items:");
                Console.WriteLine("--------------------------------------------------");
                
                if (shipment.ShipmentItems == null || !shipment.ShipmentItems.Any())
                {
                    Console.WriteLine("No items added to this shipment yet.");
                }
                else
                {
                    Console.WriteLine($"{"Product",-20} {"Quantity",-10} {"Unit",-10}");
                    foreach (var item in shipment.ShipmentItems)
                    {
                        Console.WriteLine($"{item.Product.Name,-20} {item.Quantity,-10} {item.Product.Unit,-10}");
                    }
                }
                
                Console.WriteLine("\n1. Add product");
                Console.WriteLine("2. Finish adding products");
                
                int choice = ConsoleInput.GetUserInt("Select an option: ");
                
                if (choice == 1)
                {
                    // Display products for selection
                    Console.WriteLine("\nAvailable Products:");
                    for (int i = 0; i < products.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {products[i].Name} - {products[i].Category} - Current Stock: {products[i].Stock.Quantity} {products[i].Unit}");
                    }

                    int productIndex = ConsoleInput.GetUserInt("Select product (number): ") - 1;
                    int productId = products[productIndex].ProductId;
                    
                    // Get quantity
                    int maxQuantity = shipment.ShipmentType == database.ShipmentType.Outbound ? products[productIndex].Stock.Quantity : 10000;
                    int quantity = ConsoleInput.GetUserInt($"Enter quantity (1-{maxQuantity}): ");
                    
                    try
                    {
                        // Add to shipment
                        db?.AddShipmentItem(quantity, shipment.ShipmentId, productId);
                        
                        // Refresh shipment data
                        shipment = db.GetShipmentById(shipment.ShipmentId);
                        
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
                else
                {
                    adding = false;
                }
            }
            
            EditAssignedShipment(user, shipment);
        }
        catch (Exception ex)
        {
            var error = new Error($"Error: {ex.Message}", () => EditAssignedShipment(user, shipment));
        }
    }

    internal static void MarkShipmentComplete(User user, database.Shipment shipment)
    {
        try
        {
            Console.Clear();
            Console.WriteLine($"Mark Shipment {shipment.ShipmentId} as Complete");
            Console.WriteLine(ConsoleOutput.HorizontalLine('-'));
            
            if (shipment.ShipmentItems == null || !shipment.ShipmentItems.Any())
            {
                ConsoleOutput.PrintColorMessage("Cannot complete a shipment with no items!\n", ConsoleColor.Red);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                EditAssignedShipment(user, shipment);
                return;
            }
            
            Console.WriteLine("Are you sure you want to mark this shipment as complete? (y/n)");
            var key = ConsoleInput.GetConsoleKey();
            
            if (key == ConsoleKey.Y)
            {
                db?.MarkShipmentAsDone(shipment.ShipmentId);
                ConsoleOutput.PrintColorMessage("Shipment marked as complete successfully!\n", ConsoleColor.Green);
                Console.WriteLine("Press any key to continue...");
                ConsoleInput.WaitForAnyKey();
                ManageShipmentsMenu(user);
            }
            else
            {
                EditAssignedShipment(user, shipment);
            }
        }
        catch (Exception ex)
        {
            var error = new Error($"Error: {ex.Message}", () => EditAssignedShipment(user, shipment));
        }
    }
}