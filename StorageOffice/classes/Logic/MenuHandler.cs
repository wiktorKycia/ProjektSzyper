using System;
using StorageOffice.classes.CLI;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.UsersManagement.Modules;
using StorageOffice.classes.UsersManagement.Services;

namespace StorageOffice.classes.Logic;

public class MenuHandler
{
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
        var menu = new MainMenu("Storage System manager", $"Logged in as {user.Username}", new RadioSelect(new List<RadioOption>
        {
            new RadioOption("Option 1", ()=>{DetailsMenu(user);}),
            new RadioOption("Option 2", () => {}),
            new RadioOption("Option 3", () => {})
        }));
    }
    internal static void DetailsMenu(User user)
    {
        var details = new Details("Details for Option 1", () => {MainMenu(user);});
    }
}