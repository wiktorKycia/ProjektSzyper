using System;
using System.Collections.Generic;
using System.Text;
namespace StorageOffice.classes.CLI;

public static partial class ConsoleOutput
{
    /// <summary>
    /// Creates a formatted table as a string
    /// </summary>
    /// <param name="data">Collection of string arrays representing rows of data</param>
    /// <param name="headers">Column headers</param>
    /// <returns>String representation of the table</returns>
    public static string WriteTable(IEnumerable<string[]> data, string[] headers)
    {
        StringBuilder tableString = new StringBuilder();
        
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
        tableString.AppendLine(WriteTableSeparator(columnWidths, '-'));
        tableString.AppendLine(WriteTableRow(headers, columnWidths));
        tableString.AppendLine(WriteTableSeparator(columnWidths, '-'));
        
        // Write data rows
        foreach (var row in data)
        {
            tableString.AppendLine(WriteTableRow(row, columnWidths));
        }
        
        // Write bottom separator
        tableString.AppendLine(WriteTableSeparator(columnWidths, '-'));
        
        return tableString.ToString();
    }

    private static int[] CalculateColumnWidths(IEnumerable<string[]> data, string[] headers)
    {
        var widths = headers.Select(h => h.Length).ToArray();
        
        foreach (var row in data)
        {
            for (int i = 0; i < row.Length && i < widths.Length; i++)
            {
                string cellValue = row[i] ?? "";
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

    private static string WriteTableRow(string[] cells, int[] columnWidths)
    {
        StringBuilder rowString = new StringBuilder();
        rowString.Append("|");
        
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
            rowString.Append(" " + paddedCell + "|");
        }
        
        return rowString.ToString();
    }

    private static string WriteTableSeparator(int[] columnWidths, char separatorChar)
    {
        StringBuilder separatorString = new StringBuilder();
        separatorString.Append("+");
        
        for (int i = 0; i < columnWidths.Length; i++)
        {
            separatorString.Append(Repeat(separatorChar, columnWidths[i]));
            separatorString.Append("+");
        }
        
        return separatorString.ToString();
    }
}