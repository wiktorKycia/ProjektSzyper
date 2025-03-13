using StorageOffice.classes.CLI;
namespace StorageOffice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            CheckBoxSelect select = new(new List<Option>
            {
                new Option("Option 1"),
                new Option("Option 2"),
                new Option("Option 3")
            });
            while(true)
            {
                Console.Clear();
                ConsoleOutput.PrintColorMessage("Select an option:\n\n", ConsoleColor.Cyan);
                select.DisplayOptions();
                Console.WriteLine("Press Up/Down to navigate, Enter to select, Esc to exit.");

                ConsoleKey key = ConsoleInput.GetConsoleKey();
                if (key == ConsoleKey.UpArrow)
                {
                    select.MoveUp();
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    select.MoveDown();
                }
                else if (key == ConsoleKey.Enter)
                {
                    select.SelectOption();
                }
                else if (key == ConsoleKey.Escape)
                {
                    break;
                }
                else
                {
                    ConsoleOutput.PrintColorMessage("Invalid key. Please use Up/Down to navigate.", ConsoleColor.Red);
                }
            }
        }
    }
}
