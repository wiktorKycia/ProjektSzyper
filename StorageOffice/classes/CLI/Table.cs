using System;
using System.Collections.Generic;
namespace StorageOffice.classes.CLI;

public static partial class ConsoleOutput
{
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