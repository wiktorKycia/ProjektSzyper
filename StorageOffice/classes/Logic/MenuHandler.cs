using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;


public class MenuHandler
{
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