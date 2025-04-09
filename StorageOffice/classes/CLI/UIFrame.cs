using System;

namespace StorageOffice.classes.CLI;

public static class UIFrame
{
    public static void DisplayFrame(string title, string content)
    {
        Console.WriteLine(ConsoleOutput.Header(title, '='));
        Console.WriteLine(content);
        Console.WriteLine(ConsoleOutput.HorizontalLine('='));
    }
}
