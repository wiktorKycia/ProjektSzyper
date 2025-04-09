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
    internal static string Header(string text, char Char = '-')
    {
        text = text.Margin();
        int length = (UIWidth-text.Length)/2;
        return (
            Repeat(Char, length) + 
            text +
            Repeat(Char, UIWidth - length - text.Length)
        );
    }
    internal static string CenteredText(string text, bool wrap = false)
    {
        string[] lines = text.Split('\n');

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
    internal static string Truncate(string text, int finalLength)
    {
        if(text.Length > finalLength){ return text[..(finalLength - 3)] + "...";}
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
            while(!end)
            {
                elems.Add(text.Substring(i, maxElemLength));
                i += maxElemLength;
                if(i >= text.Length) end = true;
            }
            return [..elems]; // zastępuje: elems.ToArray();
        }
    }
    
    /// <summary>
    /// Writes data as a formatted table to the console
    /// </summary>
    /// <param name="data">Collection of data rows</param>
    /// <param name="headers">Column headers</param>
    public static void WriteTable<T>(IEnumerable<T[]> data, string[] headers)
    {
        // Calculate column widths
        int[] columnWidths = CalculateColumnWidths(data, headers);
        
        // Check if table fits in the console
        int totalWidth = columnWidths.Sum() + columnWidths.Length + 1;
        if (totalWidth > UIWidth)
        {
            // Adjust column widths to fit
            AdjustColumnWidths(ref columnWidths, UIWidth);
        }
        
        // Write header row
        WriteTableSeparator(columnWidths, '-');
        WriteTableRow(headers, columnWidths);
        WriteTableSeparator(columnWidths, '-');
        
        // Write data rows
        foreach (var row in data)
        {
            string[] stringRow = [.. row.Select(item => item?.ToString() ?? "")];
            WriteTableRow(stringRow, columnWidths);
        }
        
        // Write bottom separator
        WriteTableSeparator(columnWidths, '-');
    }

    private static int[] CalculateColumnWidths<T>(IEnumerable<T[]> data, string[] headers)
    {
        var widths = headers.Select(h => h.Length).ToArray();
        
        foreach (var row in data)
        {
            for (int i = 0; i < row.Length && i < widths.Length; i++)
            {
                string cellValue = row[i]?.ToString() ?? "";
                widths[i] = Math.Max(widths[i], cellValue.Length);
            }
        }
        
        // Add padding
        return widths.Select(w => w + 2).ToArray();
    }

    private static void AdjustColumnWidths(ref int[] columnWidths, int maxTotalWidth)
    {
        int totalWidth = columnWidths.Sum() + columnWidths.Length + 1;
        if (totalWidth <= maxTotalWidth) return;
        
        // Reduce each column proportionally
        int excess = totalWidth - maxTotalWidth;
        int columnsToAdjust = columnWidths.Length;
        
        while (excess > 0 && columnsToAdjust > 0)
        {
            int reducePerColumn = Math.Max(1, excess / columnsToAdjust);
            
            for (int i = 0; i < columnWidths.Length && excess > 0; i++)
            {
                if (columnWidths[i] > 3)  // Minimum width with ellipsis
                {
                    int reduction = Math.Min(reducePerColumn, columnWidths[i] - 3);
                    columnWidths[i] -= reduction;
                    excess -= reduction;
                    
                    if (columnWidths[i] <= 3)
                    {
                        columnsToAdjust--;
                    }
                }
                else
                {
                    columnsToAdjust--;
                }
            }
        }
    }

    private static void WriteTableRow(string[] cells, int[] columnWidths)
    {
        Console.Write("|");
        
        for (int i = 0; i < cells.Length && i < columnWidths.Length; i++)
        {
            string cell = cells[i] ?? "";
            
            // Truncate if needed
            if (cell.Length > columnWidths[i] - 2)
            {
                cell = Truncate(cell, columnWidths[i] - 2);
            }
            
            // Pad the cell content
            string paddedCell = cell.PadRight(columnWidths[i] - 1);
            Console.Write(" " + paddedCell + "|");
        }
        
        Console.WriteLine();
    }

    private static void WriteTableSeparator(int[] columnWidths, char separatorChar)
    {
        Console.Write("+");
        
        for (int i = 0; i < columnWidths.Length; i++)
        {
            Console.Write(Repeat(separatorChar, columnWidths[i]));
            Console.Write("+");
        }
        
        Console.WriteLine();
    }
}

public delegate void KeyboardAction();