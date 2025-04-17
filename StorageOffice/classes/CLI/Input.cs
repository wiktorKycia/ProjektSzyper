using System;
namespace StorageOffice.classes.CLI;

public static partial class ConsoleInput
{
    public static string GetUserString(string message)
    {
        string result = "";
        while(result == "")
        {
            try
            {
                Console.Write(message);
                result = Console.ReadLine() ?? "";
            }
            catch (ArgumentNullException e)
            {
                Console.Write(e.Message);
            }
            catch (ArgumentException e)
            {
                Console.Write(e.Message);
            }
        }
        return result;
    }
}