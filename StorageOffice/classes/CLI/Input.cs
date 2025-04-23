using System;
namespace StorageOffice.classes.CLI;

/// <summary>
/// Provides utility methods for handling console input operations.
/// </summary>
public static partial class ConsoleInput
{
    /// <summary>
    /// Prompts the user for a non-empty string input.
    /// </summary>
    /// <param name="message">The message to display to the user as a prompt.</param>
    /// <returns>A non-empty string entered by the user.</returns>
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
    
    /// <summary>
    /// Prompts the user for integer input.
    /// </summary>
    /// <param name="message">The message to display to the user as a prompt.</param>
    /// <returns>An integer entered by the user.</returns>
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

    /// <summary>
    /// Prompts the user for integer input within a specified range.
    /// </summary>
    /// <param name="message">The message to display to the user as a prompt.</param>
    /// <param name="minValue">The minimum acceptable value (inclusive).</param>
    /// <param name="maxValue">The maximum acceptable value (inclusive).</param>
    /// <returns>An integer within the specified range entered by the user.</returns>
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

    /// <summary>
    /// Prompts the user for decimal input.
    /// </summary>
    /// <param name="message">The message to display to the user as a prompt.</param>
    /// <returns>A decimal number entered by the user.</returns>
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

    /// <summary>
    /// Prompts the user for date input in dd/mm/yyyy format.
    /// </summary>
    /// <param name="message">The message to display to the user as a prompt.</param>
    /// <returns>A DateTime object representing the date entered by the user.</returns>
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