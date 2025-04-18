using System;
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
            {Permission.BrowseWarehouse, new RadioOption("Warehouse", () => {DetailsMenu(user, "Warehouser");})},
            {Permission.AssignTask, new RadioOption("Tasks", () => {DetailsMenu(user, "Assign task");})},
            {Permission.DoTasks, new RadioOption("Tasks", () => {DetailsMenu(user, "Do tasks");})},
            {Permission.ManageShipments, new RadioOption("Shipments", () => {DetailsMenu(user, "Manage shipments");})},
            {Permission.BrowseShipments, new RadioOption("Shipments", () => {DetailsMenu(user, "Browse shipments");})},
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
            new RadioOption("Edit user", () => {EditUser(user);}),
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
        var addUser = new AddUser(() => {ManageUsers(user);}, user, db!);
    }

    internal static void EditUser(User user)
    {
        // var editUser = new EditUser("Edit user", () => {ManageUsers(user);});
    }

    internal static void DeleteUser(User user)
    {
        var users = PasswordManager.GetAllUsers();
        var checkBoxOptions = users.Select(u => new CheckBoxOption(u.Username, ()=>{PasswordManager.DeleteUser(u.Username);})).ToList();

        var deleteUser = new DeleteUser(new CheckBoxSelect(checkBoxOptions), () => {ManageUsers(user);});
    }

}