using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

public class MenuHandler
{
    public static void MainMenu()
    {
        // Create a menu builder
        var builder = new MenuBuilder();
        
        // Configure main menu and submenus
        builder
            .AddMenu("main", "Storage System Manager", "Main Menu")
            .AddMenu("storage", "Storage Management", "Manage your storage items", 
                () => builder.GetMenu("main").Display())
            .AddMenu("users", "User Management", "Manage system users", 
                () => builder.GetMenu("main").Display())
            .AddMenu("settings", "System Settings", "Configure system parameters", 
                () => builder.GetMenu("main").Display());
        
        // Add items to main menu
        builder
            .AddSubMenu("main", "Storage Management", "storage")
            .AddSubMenu("main", "User Management", "users")
            .AddSubMenu("main", "System Settings", "settings")
            .AddMenuItem("main", "Exit", () => Environment.Exit(0));
        
        // Add items to storage menu
        builder
            .AddMenuItem("storage", "View All Items", () => ShowMessage("Viewing all storage items..."))
            .AddMenuItem("storage", "Add New Item", () => ShowMessage("Adding new storage item..."))
            .AddMenuItem("storage", "Generate Report", () => ShowMessage("Generating storage report..."));
            
        // Add items to users menu
        builder
            .AddMenuItem("users", "View All Users", () => ShowMessage("Viewing all users..."))
            .AddMenuItem("users", "Add New User", () => ShowMessage("Adding new user..."))
            .AddMenuItem("users", "Edit User Permissions", () => ShowMessage("Editing user permissions..."));
            
        // Add items to settings menu
        builder
            .AddMenuItem("settings", "Change Theme", () => ShowMessage("Changing theme..."))
            .AddMenuItem("settings", "Configure Backup", () => ShowMessage("Configuring backup..."));
            
        // Display the main menu
        builder.GetMenu("main").Display();
    }
    
    // Helper method to show messages (would be replaced with actual functionality)
    private static void ShowMessage(string message)
    {
        Console.Clear();
        Console.WriteLine(ConsoleOutput.UIFrame("Message", ConsoleOutput.CenteredText(message)));
        Console.WriteLine("\nPress any key to continue...");
        ConsoleInput.WaitForAnyKey();
    }
}