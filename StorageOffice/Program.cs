using StorageOffice.classes.CLI;
using StorageOffice.classes.Logic;
namespace StorageOffice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            MenuHandler.MainMenu();
        }
    }
}
