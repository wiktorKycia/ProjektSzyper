namespace CLI;

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