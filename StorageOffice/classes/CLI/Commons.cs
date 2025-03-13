namespace StorageOffice.classes.CLI;

public static class ConsoleInput
{
    public static ConsoleKey GetConsoleKey()
    {
        return Console.ReadKey(true).Key;
    }
    public static void WaitForAnyKey()
    {
        Console.ReadKey(true);
    }
}

public static class ConsoleOutput
{
    public static void PrintColorMessage(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }
}