namespace StorageOffice.classes.CLI;

public static partial class ConsoleInput
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

public static partial class ConsoleOutput
{
    public static void PrintColorMessage(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }
    internal static int UIWidth => Console.WindowWidth;

    private static string Repeat(char Char, int length)
    {
        return new string(Char, length);
    }
    private static string Spaces(int length)
    {
        return Repeat(' ', length);
    }
    internal static string HorizontalLine(char Char)
    {
        return Repeat(Char, UIWidth);
    }
    internal static string Header(string text, char Char = '-')
    {
        text = text.Margin();
        int length = (UIWidth - text.Length) / 2;
        return (
            Repeat(Char, length) +
            text +
            Repeat(Char, UIWidth - length - text.Length)
        );
    }
    internal static string CenteredText(string text, bool wrap = false)
    {
        string[] lines = text.Split('\n');

        if (wrap)
        {
            List<string> wrappedLines = new();

            foreach (string line in lines)
            {
                wrappedLines.AddRange(line.DivideIntoArray(UIWidth));
            }
            lines = wrappedLines.ToArray();
        }
        string result = "";

        foreach (string line in lines)
        {
            result += Header(line, ' ');
        }
        return result;
    }
    internal static string RightAlignText(string text)
    {
        return (
            Spaces(UIWidth - text.Length) +
            text
        );
    }
    internal static string Margin(this string text, char Char = ' ', int length = 1)
    {
        return (
            Repeat(Char, length) +
            text +
            Repeat(Char, length)
        );
    }
    public static string RightMargin(this string text, char Char = ' ')
    {
        int length = UIWidth - text.Length;
        return text + Repeat(Char, length);
    }
    internal static string Truncate(string text, int finalLength)
    {
        if (text.Length > finalLength) { return text[..(finalLength - 3)] + "..."; }
        return text;
    }
    private static string[] DivideIntoArray(this string text, int maxElemLength)
    {
        if (text.Length <= maxElemLength) return [text];
        else
        {
            List<string> elems = []; // zastępuje: new List<string>();
            bool end = false;
            int i = 0;
            while (!end)
            {
                elems.Add(text.Substring(i, maxElemLength));
                i += maxElemLength;
                if (i >= text.Length) end = true;
            }
            return [.. elems]; // zastępuje: elems.ToArray();
        }
    }
    public static string UIFrame(string title, string content)
    {
        return (
            Header(title, '=') + "\n" +
            content + "\n" +
            HorizontalLine('=')
        );
    }
}

public delegate void KeyboardAction();