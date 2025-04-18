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
}