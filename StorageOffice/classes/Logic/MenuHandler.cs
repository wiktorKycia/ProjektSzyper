using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;


public class MenuHandler
{
    public static void MainMenu()
    {
        Console.OutputEncoding = System.Text.Encoding.Unicode;
        var menu = new Menu("Storage System manager", "Welcome", new RadioSelect(new List<RadioOption>
        {
            new RadioOption("Option 1"),
            new RadioOption("Option 2"),
            new RadioOption("Option 3")
        }));
        menu.Run();
    }
}