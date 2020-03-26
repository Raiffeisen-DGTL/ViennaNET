using System;
using System.IO;
using System.Linq;
using System.Text;
using ViennaNET.Excel.Contracts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ViennaNET.Excel.Impl
{
  public class OpenXmlExcelProcessor : IExcelProcessor
  {
    private const string dimesionReference = "A1:BV1000";
    private const string formatColor = "FFFFFF00";
    private const string formatReference = "C11:C1000";
    private const string formatFormula = "IF(AT11<>\"\",IF(COUNTIF($AT$11:$AT$1000,AT11)>1,TRUE,FALSE),FALSE)";

    public void AddConditionalFormatting(string fileName)
    {
      if (fileName == null)
      {
        throw new ArgumentNullException(nameof(fileName));
      }

      using (var excel = SpreadsheetDocument.Open(fileName, true))
      {
        var sheet = excel.WorkbookPart.Workbook.Descendants<Sheet>()
                         .First();
        var ws = ((WorksheetPart)excel.WorkbookPart.GetPartById(sheet.Id)).Worksheet;

        var sheetData = ws.GetFirstChild<SheetData>();
        AddRows(sheetData, 1000, 3);

        ws.SheetDimension.Reference = dimesionReference;

        var style = excel.WorkbookPart.WorkbookStylesPart.Stylesheet;
        var colorIndex = AddColor(style, formatColor);

        AddConditionalFormatting(ws, colorIndex);
      }
    }

    public void RemoveInvalidUrls(Stream stream)
    {
      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }

      using (var excel = SpreadsheetDocument.Open(stream, true))
      {
        var sheet = excel.WorkbookPart.Workbook.Descendants<Sheet>()
                         .First();
        var ws = ((WorksheetPart)excel.WorkbookPart.GetPartById(sheet.Id)).Worksheet;
        var links = ws.Descendants<Hyperlinks>()
                      .FirstOrDefault();
        if (links != null)
        {
          var sheetData = ws.GetFirstChild<SheetData>();
          var sharedStrings = excel.WorkbookPart.SharedStringTablePart.SharedStringTable;
          foreach (var hyperlink in links.Descendants<Hyperlink>()
                                         .ToList())
          {
            ProcessUrl(sheetData, sharedStrings, hyperlink);
          }
          if (!links.Descendants<Hyperlink>()
                    .Any())
          {
            links.Remove();
          }
        }
      }
    }

    private static bool InvalidUrl(string url)
    {
      return Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _);
    }

    private static string GetSharedString(SharedStringTable table, int index)
    {
      var item = table.Descendants<SharedStringItem>()
                      .Skip(index)
                      .FirstOrDefault();
      if (item != null)
      {
        return item.Text.InnerText;
      }
      return string.Empty;
    }

    private static int ParseRow(string excelRef)
    {
      var num = new StringBuilder();
      foreach (var ch in excelRef)
      {
        if (Char.IsNumber(ch))
        {
          num.Append(ch);
        }
      }
      int rowNum;
      if (int.TryParse(num.ToString(), out rowNum))
      {
        return rowNum;
      }
      return -1;
    }

    private static void ProcessUrl(SheetData sheetData, SharedStringTable sharedStrings, Hyperlink hyperlink)
    {
      var reference = hyperlink.Reference;
      var rowNumber = ParseRow(reference);
      if (rowNumber != -1)
      {
        var row = sheetData.Descendants<Row>()
                           .FirstOrDefault(r => r.RowIndex == rowNumber);
        if (row != null)
        {
          var cell = row.Descendants<Cell>()
                        .FirstOrDefault(c => c.CellReference.Value == reference);
          if (cell != null)
          {
            string url;
            if (cell.DataType.Value == CellValues.SharedString)
            {
              var index = int.Parse(cell.CellValue.InnerText);
              url = GetSharedString(sharedStrings, index);
            }
            else if (cell.DataType.Value == CellValues.InlineString)
            {
              url = cell.CellValue.InnerText;
            }
            else
            {
              return;
            }
            if (InvalidUrl(url))
            {
              hyperlink.Remove();
            }
          }
        }
      }
    }

    private void AddConditionalFormatting(Worksheet ws, int index)
    {
      var conditionalFormatting1 =
        new ConditionalFormatting { SequenceOfReferences = new ListValue<StringValue> { InnerText = formatReference } };
      var conditionalFormattingRule1 =
        new ConditionalFormattingRule { Type = ConditionalFormatValues.Expression, FormatId = (uint)index, Priority = 1 };
      var formula1 = new Formula { Text = formatFormula };
      conditionalFormattingRule1.Append(formula1);
      conditionalFormatting1.Append(conditionalFormattingRule1);
      var pp = ws.Descendants<PhoneticProperties>()
                 .FirstOrDefault();
      ws.InsertAfter(conditionalFormatting1, pp);
    }

    private static string GetExcelColumnName(int columnNumber)
    {
      var dividend = columnNumber;
      var columnName = String.Empty;

      while (dividend > 0)
      {
        var modulo = (dividend - 1) % 26;
        columnName = Convert.ToChar(65 + modulo) + columnName;
        dividend = ((dividend - modulo) / 26);
      }

      return columnName;
    }

    private static int AddColor(Stylesheet stylesheet, string color)
    {
      var diff1 = new DifferentialFormat(new Fill(new PatternFill(new BackgroundColor { Rgb = color })));
      stylesheet.DifferentialFormats.AppendChild(diff1);
      var count = stylesheet.DifferentialFormats.Descendants<DifferentialFormat>()
                            .Count();
      stylesheet.DifferentialFormats.Count = (uint)count;
      return count - 1;
    }

    private static void AddRows(OpenXmlCompositeElement sheet, int maxCount, int columns)
    {
      var maxIndex = sheet.Elements<Row>()
                          .Select(r => r.RowIndex.Value).Max();
      var count = sheet.Elements<Row>()
                       .Count();
      for (var i = count; i <= maxCount; i++)
      {
        var row = new Row { RowIndex = ++maxIndex };

        for (var j = 1; j < columns + 1; j++)
        {
          var cell = new Cell
          {
            DataType = CellValues.String,
            CellValue = new CellValue(string.Empty),
            CellReference = GetExcelColumnName(j) + row.RowIndex
          };
          row.AppendChild(cell);
        }
        sheet.AppendChild(row);
      }
    }
  }
}
