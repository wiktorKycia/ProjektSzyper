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

    public static int GetUserInt(string message, int minValue, int maxValue)
    {
        int result = 0;
        bool valid = false;
        while(!valid)
        {
            Console.Write(message);
            string input = Console.ReadLine() ?? "";
            valid = int.TryParse(input, out result) && result >= minValue && result <= maxValue;
            if (!valid)
            {
                Console.WriteLine($"Invalid input. Please enter a number between {minValue} and {maxValue}.");
            }
        }
        return result;
    }

    public static decimal GetUserDecimal(string message)
    {
        decimal result = 0;
        bool valid = false;
        while(!valid)
        {
            Console.Write(message);
            string input = Console.ReadLine() ?? "";
            valid = decimal.TryParse(input, out result);
            if (!valid)
            {
                Console.WriteLine("Invalid input. Please enter a decimal number.");
            }
        }
        return result;
    }

    public static DateTime GetUserDate(string message)
    {
        DateTime result = DateTime.MinValue;
        bool valid = false;
        while(!valid)
        {
            Console.Write(message + " (dd/mm/yyyy): ");
            string input = Console.ReadLine() ?? "";
            valid = DateTime.TryParse(input, out result);
            if (!valid)
            {
                Console.WriteLine("Invalid input. Please enter a valid date.");
            }
        }
        return result;
    }
}