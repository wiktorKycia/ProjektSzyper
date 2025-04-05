using StorageOffice.classes.CLI;
using StorageOffice.classes.database;
namespace StorageOffice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var db = new StorageContext()) { }
        }
    }
}
