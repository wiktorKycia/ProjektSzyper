using System;
namespace StorageOffice.classes.CLI;

public static partial class ConsoleInput
{
    public static string GetUserString(string message)
    {
        string result = "";
        while(result == "")
        {
            Console.Write(message);
            result = Console.ReadLine() ?? "";
        }
        return result;
    }
    public static int GetUserInt(string message)
    {
        int result = 0;
        bool valid = false;
        while(!valid)
        {
            Console.Write(message);
            string input = Console.ReadLine() ?? "";
            valid = int.TryParse(input, out result);
            if (!valid)
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }
        return result;
    }
}