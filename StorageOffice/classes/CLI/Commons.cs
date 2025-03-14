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
    public static void PrintCenteredMessage(string message, char sign = ' ')
    {
        int windowWidth = Console.WindowWidth;
        int messageLength = message.Length;
        int spaces = (windowWidth - messageLength) / 2;
        Console.WriteLine(new string(sign, spaces) + ' ' + message + ' ' + new string(sign, spaces));
    }
    public static void PrintCenteredMessage(string message, ConsoleColor color, char sign = ' ')
    {
        Console.ForegroundColor = color;
        PrintCenteredMessage(message, sign);
        Console.ResetColor();
    }

    public static void HorizontalLine(char sign = '-')
    {
        Console.WriteLine(new string(sign, Console.WindowWidth));
    }
}

public static class ConsoleSize
{
    public static int GetConsoleWidth()
    {
        return Console.WindowWidth;
    }
    public static int GetConsoleHeight()
    {
        return Console.WindowHeight;
    }
}