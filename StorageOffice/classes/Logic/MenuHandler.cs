using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

public class MenuHandler
{
    public static void LoginMenu()
    {
        bool isAnyUserCreated = ;
        if (!isAnyUserCreated)
        {
            // Console.WriteLine("No user has been created on the system, so the details currently provided will be used to create the first administrator. Enter the details for him/her.\n");
        }

        var loginMenu = new Login(
            title: "Login menu",
            heading: "Welcome to the logistics warehouse management system!",
            firstUser: !PasswordManager.CheckFile()
        );
    }
    public static void MainMenu()
    {
        var menu = new MainMenu("Storage System manager", "Welcome", new RadioSelect(new List<RadioOption>
        {
            new RadioOption("Option 1", DetailsMenu),
            new RadioOption("Option 2", () => {}),
            new RadioOption("Option 3", () => {})
        }));
    }
    public static void DetailsMenu()
    {
        var details = new Details("Details for Option 1", MainMenu);
    }
}