using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ViennaNET.Word
{
  internal class WordTableWriter
  {
    public static void Write(IEnumerable<object> rows, Table table, bool skipHeader)
    {
      var rowIndex = 1;
      var headerWritten = false;

      foreach (var row in rows)
      {
        var tableRow = new TableRow();

        table.Append(tableRow);

        var properties = row.GetType().GetProperties();

        if (!skipHeader && !headerWritten)
        {
          WriteHeader(properties, tableRow);

          headerWritten = true;
        }
        else
        {
          WriteCells(properties, tableRow, row);
        }

        rowIndex++;
      }
    }

    private static void WriteCells(IEnumerable<PropertyInfo> properties, TableRow targetRow, object sourceRow)
    {
      var columnIndex = 0;

      foreach (var item in properties)
      {
        var value = item.GetValue(sourceRow)?.ToString();
        var newCell = new TableCell(new Paragraph(new Run(new Text(value))));

        targetRow.InsertAt(new TableCell(new Paragraph(new Run(new Text(value)))), columnIndex);

        columnIndex++;
      }
    }

    private static void WriteHeader(IEnumerable<PropertyInfo> properties, TableRow targetRow)
    {
      var columnIndex = 0;

      foreach (var item in properties)
      {
        var headerName = item.Name;

        var displayNameAttr = item.GetCustomAttributes(typeof(DisplayNameAttribute), true)
          .Cast<DisplayNameAttribute>()
          .FirstOrDefault();

        if (displayNameAttr != null)
        {
          headerName = displayNameAttr.DisplayName;
        }

        targetRow.InsertAt(new TableCell(new Paragraph(new Run(new Text(headerName)))), columnIndex);

        columnIndex++;
      }
    }
  }
}