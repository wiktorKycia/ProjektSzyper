using StorageOffice.classes.CLI;
using StorageOffice.classes.Logic;
namespace StorageOffice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var menu = new Menu("Storage Office", "Welcome to Storage Office", new RadioSelect([
                new RadioOption("Option 1"),
                new RadioOption("Option 2"),
                new RadioOption("Option 3")
            ]));
            menu.Run();
        }
    }
}
