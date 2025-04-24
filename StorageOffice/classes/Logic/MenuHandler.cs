using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Models;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

/// <summary>
/// Handles the main menu and submenus for the application.
/// Provides methods to navigate through different functionalities.
/// </summary>
/// <remarks>
/// This class is responsible for managing the user interface and interactions.
/// It includes methods for user authentication, task assignment, shipment management,
/// warehouse browsing, and user management.
/// </remarks>
public static class MenuHandler
{
    /// <summary>
    /// Database instance for managing storage data.
    /// </summary>
    public static database.StorageDatabase? db = null;

    /// <summary>
    /// Initializes the menu system and starts the application.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user.</param>
    internal static void Start(User user)
    {
        if(PasswordManager.CheckFile())
        {
            var loginMenu = new Login(
                title: "Login menu",
                heading: "Welcome to the logistics warehouse management system!",
                onExit: () => {MainMenu(user);},
                user: user
            );
        }
        else
        {
            var firstUserMenu = new FirstUser(
                title: "Register first user",
                heading: "No user has been created on the system, so the details currently provided will be used to create the first administrator. Enter the details for him/her.\n",
                onExit: () => {MainMenu(user);},
                user: user
            );
        }
    }

    /// <summary>
    /// Displays the main menu for the application.
    /// This menu provides options based on the user's permissions.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user.</param>
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


        var menu = new MainMenu("Storage System manager", $"Logged in as {user.Username}", new RadioSelect(radioOptions), ()=>Start(user));
    }

    /// <summary>
    /// Displays the details menu based on the selected option.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user.</param>
    internal static void DetailsMenu(User user, string _details)
    {
        if (_details == "Warehouse")
        {
            BrowseWarehouse(user);
            return;
        }
        else if (_details == "Assign task")
        {
            AssignTask(user);
            return; 
        }
        else if (_details == "Do tasks")
        {
            DoTasks(user);
            return;
        }
        
        var details = new Details(_details, () => {MainMenu(user);});
    }

    // Task Management Methods

    /// <summary>
    /// Displays the task assignment menu for the user.
    /// This menu allows the user to assign tasks to other users.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user.</param>
    internal static void AssignTask(User user)
    {
        var assignTaskMenu = new AssignTaskMenu(user, () => MainMenu(user));
    }

    /// <summary>
    /// Displays the task completion menu for the user.
    /// This menu allows the user to complete tasks assigned to them.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user.</param>
    internal static void DoTasks(User user)
    {
        var doTasksMenu = new DoTasksMenu(user, () => MainMenu(user));
    }

    // Warehouse Browsing Methods

    /// <summary>
    /// Displays the warehouse browsing menu for the user.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    internal static void BrowseWarehouse(User user)
    {
        var browseWarehouseMenu = new BrowseWarehouseMenu(user, () => MainMenu(user));
    }

    /// <summary>
    /// Displays the logs menu for the user.
    /// This menu allows the user to view logs of actions performed in the system.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
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

    /// <summary>
    /// Displays the user management menu for the user.
    /// This menu allows the user to manage other users in the system.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
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

    /// <summary>
    /// Displays the menu for viewing users.
    /// This menu allows the user to see a list of all users in the system.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    internal static void ViewUsers(User user)
    {
        var users = PasswordManager.GetAllUsers();
        var viewUsers = new Users(users, () => {ManageUsers(user);});
    }

    /// <summary>
    /// Displays the menu for adding a new user.
    /// This menu allows the user to enter details for a new user.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    internal static void AddUser(User user)
    {
        var addUser = new AddUser(() => {ManageUsers(user);}, user);
    }

    /// <summary>
    /// Displays the menu for deleting users.
    /// This menu allows the user to select users to delete.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    internal static void DeleteUser(User user)
    {
        var users = PasswordManager.GetAllUsers();
        var checkBoxOptions = users.Select(u => new CheckBoxOption(u.Username, ()=>{
            PasswordManager.DeleteUser(u.Username);
            db!.DeleteUser(db.GetUserIdByUsername(u.Username));
        })).ToList();

        var deleteUser = new DeleteUser(new CheckBoxSelect(checkBoxOptions), () => {ManageUsers(user);});
    }

    /// <summary>
    /// Displays the menu for editing users.
    /// This menu allows the user to select a user to edit.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    internal static void EditUserMenu(User user)
    {
        var users = PasswordManager.GetAllUsers();
        var radioOptions = users.Select(u => new RadioOption(u.Username, () => {
            EditConcreteUser(user, u.Username);
        })).ToList();

        var editUser = new EditUser("Edit user", new RadioSelect(radioOptions), () => {ManageUsers(user);});
    }

    /// <summary>
    /// Displays the menu for editing a specific user.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    /// <param name="username">The username of the user to be edited</param>
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

    /// <summary>
    /// Displays the menu for editing a user's username.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    /// <param name="username">The username of the user to be edited</param>
    internal static void EditUsername(User user, string username)
    {
        var editUsername = new EditUsername(username, () => {ManageUsers(user);});
    }
    /// <summary>
    /// Displays the menu for editing a user's password.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    /// <param name="username">The username of the user whose password is to be edited</param>
    internal static void EditPassword(User user, string username)
    {
        var editPassword = new EditPassword(username, () => {ManageUsers(user);});
    }
    /// <summary>
    /// Displays the menu for editing a user's role.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    /// <param name="username">The username of the user whose role is to be edited</param>
    internal static void EditRole(User user, string username)
    {
        var editRole = new EditRole(username, () => {ManageUsers(user);});
    }

    /// <summary>
    /// Displays the shipment management menu for the user.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    internal static void ManageShipmentsMenu(User user)
    {
        var manageShipments = new ManageShipments(new RadioSelect(new List<RadioOption>
        {
            new RadioOption("Create Inbound Shipment (Import)", () => AddShipment(user, database.ShipmentType.Inbound)),
            new RadioOption("Create Outbound Shipment (Export)", () => AddShipment(user, database.ShipmentType.Outbound)),
            new RadioOption("View All Shipments", () => ViewAllShipments(user)),
            new RadioOption("View Pending Shipments", () => ViewPendingShipments(user)),
            new RadioOption("View Completed Shipments", () => ViewCompletedShipments(user))
        }), () => MainMenu(user));
    }

    /// <summary>
    /// Displays the shipment browsing menu for the user.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    internal static void BrowseShipmentsMenu(User user)
    {
        var browseShipments = new ManageShipments(new RadioSelect(new List<RadioOption>
        {
            new RadioOption("View All Shipments", () => ViewAllShipments(user)),
            new RadioOption("View Pending Shipments", () => ViewPendingShipments(user)),
            new RadioOption("View Completed Shipments", () => ViewCompletedShipments(user))
        }), () => MainMenu(user));
    }

    /// <summary>
    /// Displays the shipment addition menu for the user.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    /// <param name="shipmentType">The type of shipment to be added (Inbound or Outbound)</param>
    internal static void AddShipment(User user, database.ShipmentType shipmentType)
    {
        var addShipment = new AddShipment(() => ManageShipmentsMenu(user), user, shipmentType);
    }

    /// <summary>
    /// Displays the menu for viewing all shipments.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    internal static void ViewAllShipments(User user)
    {
        var shipments = db?.GetAllShipments() ?? new List<database.Shipment>();
        var viewShipments = new ViewShipments(shipments, () => ManageShipmentsMenu(user), "All Shipments");
    }

    /// <summary>
    /// Displays the menu for viewing pending shipments.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    internal static void ViewPendingShipments(User user)
    {
        var shipments = db?.GetNotCompletedShipments() ?? new List<database.Shipment>();
        var viewShipments = new ViewShipments(shipments, () => ManageShipmentsMenu(user), "Pending Shipments", showPendingOnly:true);
    }

    /// <summary>
    /// Displays the menu for viewing completed shipments.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    internal static void ViewCompletedShipments(User user)
    {
        var allShipments = db?.GetAllShipments() ?? new List<database.Shipment>();
        var completedShipments = allShipments.Where(s => s.IsCompleted).ToList();
        var viewShipments = new ViewShipments(completedShipments, () => ManageShipmentsMenu(user), "Completed Shipments", showCompletedOnly:true);
    }

    /// <summary>
    /// Displays the menu for managing assigned shipments.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    internal static void ManageAssignedShipments(User user)
    {
        try 
        {
            // Retrieve the user ID from the database based on the username.
            int userId = db?.GetUserIdByUsername(user.Username) ?? 0;
            if (userId == 0)
            {
                throw new InvalidOperationException("User not found in database");
            }
            
            // Retrieve a list of not completed shipments assigned to the current user.
            var shipments = db?.GetNotCompletedShipmentsAssignedToUser(userId) ?? new List<database.Shipment>();
            
            // Check if the user has any pending shipments assigned to them.
            if (!shipments.Any())
            {
                // If there are no pending shipments, display an error message.
                var error = new Error("You don't have any pending shipments assigned to you.", () => ManageShipmentsMenu(user));
                return;
            }
            
            // Create a list of RadioOption for each assigned shipment.
            var shipmentOptions = shipments.Select(s => new RadioOption(
                $"ID: {s.ShipmentId} - {s.ShipmentType} - {(s.ShipmentType == database.ShipmentType.Inbound ? s.Shipper?.Name : s.Shop?.ShopName)}", 
                () => EditAssignedShipment(user, s)
            )).ToList();
            
            // Display a menu to select a shipment to edit.
            var selectShipment = new EditUser("Your Assigned Shipments", new RadioSelect(shipmentOptions), () => ManageShipmentsMenu(user));
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during the process.
            var error = new Error($"Error: {ex.Message}", () => ManageShipmentsMenu(user));
        }
    }

    /// <summary>
    /// Displays the menu for editing an assigned shipment.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    /// <param name="shipment">The shipment object to be edited</param>
    internal static void EditAssignedShipment(User user, database.Shipment shipment)
    {
        var shipmentOptions = new List<RadioOption>
        {
            new RadioOption("Add Products", () => AddProductsToShipment(user, shipment)),
            new RadioOption("Mark as Complete", () => MarkShipmentComplete(user, shipment))
        };
        
        var editShipment = new EditShipment(shipment, new RadioSelect(shipmentOptions), () => ManageAssignedShipments(user), user);
    }

    /// <summary>
    /// Displays the menu for adding products to a shipment.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    /// <param name="shipment">The shipment object to which products will be added</param>
    internal static void AddProductsToShipment(User user, database.Shipment shipment)
    {
        var productManager = new ShipmentProductManager(shipment, () => EditAssignedShipment(user, shipment), user);
    }

    /// <summary>
    /// Marks a shipment as complete.
    /// </summary>
    /// <param name="user">The user object representing the logged-in user</param>
    /// <param name="shipment">The shipment object to be marked as complete</param>
    internal static void MarkShipmentComplete(User user, database.Shipment shipment)
    {
        var completeManager = new CompleteShipment(shipment, () => ManageShipmentsMenu(user), () => EditAssignedShipment(user, shipment));
    }
}