namespace StorageOffice.classes.CLI;

/// <summary>
/// Provides utility methods for handling console input operations.
/// </summary>
public static partial class ConsoleInput
{
    /// <summary>
    /// Reads a key press from the console without displaying it and returns the key pressed.
    /// </summary>
    /// <returns>
    /// The <see cref="ConsoleKey"/> value representing the key that was pressed.
    /// </returns>
    public static ConsoleKey GetConsoleKey()
    {
        return Console.ReadKey(true).Key;
    }

    /// <summary>
    /// Waits for the user to press any key without displaying the key press in the console.
    /// </summary>
    public static void WaitForAnyKey()
    {
        Console.ReadKey(true);
    }
}

public static partial class ConsoleOutput
{
    /// <summary>
    /// Prints a message to the console with the specified color.
    /// </summary>
    public static void PrintColorMessage(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }

    internal static int UIWidth => Console.WindowWidth;

    /// <summary>
    /// Repeats a character a specified number of times.
    /// </summary>
    /// <param name="Char">The character to repeat.</param>
    /// <param name="length">The number of times to repeat the character.</param>
    /// <returns>A string consisting of the repeated character.</returns>
    private static string Repeat(char Char, int length)
    {
        return new string(Char, length);
    }

    /// <summary>
    /// Creates a string of spaces of a specified length.
    /// </summary>
    /// <param name="length">The length of the string of spaces.</param>
    /// <returns>A string consisting of spaces.</returns>
    private static string Spaces(int length)
    {
        return Repeat(' ', length);
    }

    /// <summary>
    /// Creates a horizontal line of a specified character.
    /// </summary>
    /// <param name="Char">The character to use for the line.</param>
    /// <returns>A string representing the horizontal line.</returns>
    internal static string HorizontalLine(char Char)
    {
        return Repeat(Char, UIWidth);
    }

    /// <summary>
    /// Creates a header string with a specified text and character.
    /// </summary>
    /// <param name="text">The text to include in the header.</param>
    /// <param name="Char">The character to use for the header line.</param>
    /// <returns>A string representing the header.</returns>
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

    /// <summary>
    /// Creates a centered text string with optional wrapping.
    /// </summary> 
    /// <param name="text">The text to center.</param>
    /// <param name="wrap">Whether to wrap the text if it exceeds the console width.</param>
    /// <returns>A string representing the centered text.</returns>
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

    /// <summary>
    /// Creates a right-aligned text string.
    /// </summary>
    /// <param name="text">The text to right-align.</param>
    /// <returns>A string representing the right-aligned text.</returns>
    internal static string RightAlignText(string text)
    {
        return (
            Spaces(UIWidth - text.Length) +
            text
        );
    }

    /// <summary>
    /// Creates a margin for the text with a specified character.
    /// </summary>
    /// <param name="text">The text to add a margin to.</param>
    /// <param name="Char">The character to use for the margin.</param>
    /// <param name="length">The length of the margin.</param>
    /// <returns>A string representing the text with a margin.</returns>
    internal static string Margin(this string text, char Char = ' ', int length = 1)
    {
        return (
            Repeat(Char, length) +
            text +
            Repeat(Char, length)
        );
    }

    /// <summary>
    /// Creates a right margin for the text.
    /// </summary>
    /// <param name="text">The text to right-align.</param>
    /// <param name="Char">The character to use for the right margin.</param>
    /// <returns>A string representing the right-aligned text with a margin.</returns>
    public static string RightMargin(this string text, char Char = ' ')
    {
        int length = UIWidth - text.Length;
        return text + Repeat(Char, length);
    }

    /// <summary>
    /// Truncates a string to a specified length and appends ellipsis if necessary.
    /// </summary>
    /// <param name="text">The text to truncate.</param>
    /// <param name="finalLength">The maximum length of the truncated string.</param>
    /// <returns>A string representing the truncated text.</returns>
    internal static string Truncate(string text, int finalLength)
    {
        if (text.Length > finalLength) { return text[..(finalLength - 3)] + "..."; }
        return text;
    }

    /// <summary>
    /// Divides a string into an array of substrings of a specified maximum length.
    /// </summary>
    /// <param name="text">The text to divide.</param>
    /// <param name="maxElemLength">The maximum length of each substring.</param>
    /// <returns>An array of strings representing the divided text.</returns>
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

    /// <summary>
    /// Creates a user interface frame with a title and content.
    /// </summary>
    /// <param name="title">The title of the frame.</param>
    /// <param name="content">The content to display in the frame.</param>
    /// <returns>A string representing the user interface frame.</returns>
    /// <remarks>
    /// This method creates a frame with a title and content, using the specified character for the frame border.
    /// The title is centered within the frame, and the content is displayed below the title.
    /// The frame is closed with a horizontal line of the same character.
    /// </remarks>
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