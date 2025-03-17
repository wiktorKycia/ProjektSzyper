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
    internal static string Header(string Str, char Char = '-')
    {
        Str = Str.Margin();
        int length = (UIWidth-Str.Length)/2;
        return (
            Repeat(Char, length) + 
            Str +
            Repeat(Char, UIWidth - length - Str.Length)
        );
    }
    internal static string CenteredText(string Str, bool wrap = false)
    {
        string[] lines = Str.Split('\n');

        if(wrap)
        {
            List<string> wrappedLines = new();

            foreach(string line in lines)
            {
                wrappedLines.AddRange(line.DivideIntoArray(UIWidth));
            }
            lines = wrappedLines.ToArray();
        }
        string result = "";

        foreach(string line in lines)
        {
            result += Header(line, ' ') + '\n';
        }
        return result;
    }
    internal static string RightAlignText(string Str)
    {
        return (
            Spaces(UIWidth - Str.Length) +
            Str
        );
    }
    internal static string Margin(this string Str, char Char = ' ', int lenght = 1)
    {
        return (
            Repeat(Char, lenght) + 
            Str + 
            Repeat(Char, lenght)
        );
    }
    internal static string Truncate(string Str, int finalLength)
    {
        if(Str.Length > finalLength){ return Str[..(finalLength - 3)] + "...";}
        return Str;
    }
    private static string[] DivideIntoArray(this string Str, int maxElemLength)
    {
        if (Str.Length <= maxElemLength) return [Str];
        else
        {
            List<string> elems = new();
            bool end = false;
            int i = 0;
            while(!end)
            {
                elems.Add(Str.Substring(i, maxElemLength));
                i += maxElemLength;
                if(i >= Str.Length) end = true;
            }
            return elems.ToArray();
        }
    }

}