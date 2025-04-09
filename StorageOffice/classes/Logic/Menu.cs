using System;
using StorageOffice.classes.CLI;
namespace StorageOffice.classes.Logic;

public class Menu
{
    public readonly string Title;
    public readonly string Heading;
    public Select Select { get; set; }
    public Dictionary<ConsoleKey, KeyboardAction> KeyboardActions { get; set; }
    
}