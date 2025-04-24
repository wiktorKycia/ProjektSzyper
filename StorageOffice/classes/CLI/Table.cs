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

    /// <summary>
    /// Calculates the maximum width of each column based on the data and headers
    /// </summary>
    /// <param name="data">Collection of string arrays representing rows of data</param>
    /// <param name="headers">Column headers</param>
    /// <returns>An array of integers representing the width of each column</returns>
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

    /// <summary>
    /// Adjusts the width of table columns to fit within a maximum total width.
    /// </summary>
    /// <param name="columnWidths">Array of column widths to be adjusted. Modified in-place.</param>
    /// <param name="maxTotalWidth">The maximum allowed width for the entire table, including borders.</param>
    /// <remarks>
    /// The algorithm reduces column widths proportionally when the total width exceeds the maximum allowed width.
    /// It preserves a minimum width of 3 characters per column to allow for ellipsis display.
    /// Columns are reduced iteratively until the desired total width is achieved or further reduction is not possible.
    /// </remarks>
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
                int reduction = Math.Min(reducePerColumn, columnWidths[i] - 3);
                if (columnWidths[i] - reduction > 5)  // Minimum width with ellipsis
                {
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

    /// <summary>
    /// Creates a formatted table row string from an array of cell values.
    /// </summary>
    /// <param name="cells">Array of string values representing cell contents.</param>
    /// <param name="columnWidths">Array of integers representing the width of each column.</param>
    /// <returns>A string representing a formatted table row with proper cell padding and borders.</returns>
    /// <remarks>
    /// The method handles truncation of cell contents that exceed their column width,
    /// ensuring that all cells fit within their allocated space. Each cell is padded 
    /// with spaces to maintain consistent column widths throughout the table.
    /// </remarks>
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

    /// <summary>
    /// Creates a horizontal separator line for a table using the specified character.
    /// </summary>
    /// <param name="columnWidths">Array of integers representing the width of each column.</param>
    /// <param name="separatorChar">The character to use for the separator line (typically '-' or '=').</param>
    /// <returns>A string representing a horizontal separator line with proper column widths and junction characters.</returns>
    /// <remarks>
    /// The method creates a separator line with '+' characters at column junctions
    /// and the specified separator character filling the width of each column.
    /// </remarks>
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