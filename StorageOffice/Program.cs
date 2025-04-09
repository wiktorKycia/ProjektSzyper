using StorageOffice.classes.CLI;
using StorageOffice.classes.Logic;
namespace StorageOffice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var menu = new Menu("Storage Office", "Welcome to Storage Office", new RadioSelect([
                new RadioOption("Option 1"),
                new RadioOption("Option 2"),
                new RadioOption("Option 3")
            ]));
            menu.Run();
        }
    }
}
