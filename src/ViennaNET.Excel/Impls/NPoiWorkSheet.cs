using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using ViennaNET.Excel.Contracts;

namespace ViennaNET.Excel.Impl
{
  internal class NPoiWorkSheet : IWorkSheet
  {
    private static readonly string[] vbaSheetNames = { "Module", "Модуль" };
    private readonly ISheet _sheet;

    public NPoiWorkSheet(ISheet sheet, NPoiExcelFile excel)
    {
      _sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
      ExcelFile = excel ?? throw new ArgumentNullException(nameof(excel));
    }

    internal NPoiExcelFile ExcelFile { get; }

    public string Name => _sheet.SheetName;

    public IExcelRow this[int i]
    {
      get
      {
        var row = _sheet.GetRow(i) ?? _sheet.CreateRow(i);
        return new NPoiRow(row, this);
      }
    }

    public IEnumerable<IExcelRow> Rows()
    {
      for (var i = _sheet.GetRowEnumerator(); i.MoveNext();)
      {
        if (i.Current != null)
        {
          yield return new NPoiRow(i.Current as IRow, this);
        }
      }
    }

    public void HideColumn(int column)
    {
      _sheet.SetColumnHidden(column, true);
    }

    public void ShowColumn(int column)
    {
      _sheet.SetColumnHidden(column, false);
    }

    public bool ColumnHidden(int column)
    {
      try
      {
        return _sheet.IsColumnHidden(column);
      }
      catch
      {
        return false;
      }
    }

    public int GetColumnWidth(int column)
    {
      return _sheet.GetColumnWidth(column);
    }

    public void CopyStyles(
      int fromRow, int fromColumn, int startRow, int startColumn, int endRow, int endColumn, string dataFormat = null)
    {
      var cellStyle = _sheet.GetRow(fromRow)
        .GetCell(fromColumn)
        .CellStyle;
      var newStyle = _sheet.Workbook.CreateCellStyle();
      newStyle.CloneStyleFrom(cellStyle);
      if (!string.IsNullOrEmpty(dataFormat))
      {
        var format = ExcelFile.Excel.CreateDataFormat()
          .GetFormat(dataFormat);
        newStyle.DataFormat = format;
      }

      for (var r = startRow; r < endRow + 1; r++)
      {
        for (var c = startColumn; c < endColumn + 1; c++)
        {
          var row = _sheet.GetRow(r) ?? _sheet.CreateRow(r);
          if (row != null)
          {
            var cell = row.GetCell(c) ?? row.CreateCell(c);
            cell.CellStyle = newStyle;
          }
        }
      }
    }

    public void SetColumnWidth(int column, int width)
    {
      _sheet.SetColumnWidth(column, width);
    }

    public void ShiftDownRow(int row, int rowNumber = 1)
    {
      _sheet.ShiftRows(row, _sheet.LastRowNum, rowNumber, true, false);
    }

    public void RecalculateFormulas()
    {
      _sheet.ForceFormulaRecalculation = true;
      var helper = ExcelFile.Excel.GetCreationHelper();
      var evaluator = helper.CreateFormulaEvaluator();
      evaluator.EvaluateAll();
    }

    public void HideRow(int rowNumber)
    {
      var row = _sheet.GetRow(rowNumber);
      row.Height = 0;
    }

    public object ReadCellByName(string name)
    {
      var nameObj = ExcelFile.Excel.GetName(name);
      if (nameObj == null)
      {
        return null;
      }

      if (nameObj.RefersToFormula.Contains("#REF!"))
      {
        return null; // bad formula
      }

      var refs = AreaReference.GenerateContiguous(nameObj.RefersToFormula);
      if (refs == null || refs.Length <= 0)
      {
        return null;
      }

      var cellRef = refs[0]
        .FirstCell;
      if (cellRef.SheetName.Contains("#REF"))
      {
        return null;
      }

      var sheet = ExcelFile[cellRef.SheetName];
      return sheet[cellRef.Row][cellRef.Col];
    }

    public void CreateVerticalRange(string name, int cellIndex, int startRow, int endRow)
    {
      if (startRow > endRow)
      {
        throw new InvalidOperationException("CreateRange - startRow can not be greater to endRow");
      }

      var namedRange = GetName(name);
      var forumla = new StringBuilder();
      forumla.Append(Name)
        .Append("!$")
        .Append(CellReference.ConvertNumToColString(cellIndex))
        .Append("$")
        .Append(startRow + 1);
      if (endRow > startRow)
      {
        forumla.Append(":$")
          .Append(CellReference.ConvertNumToColString(cellIndex))
          .Append("$")
          .Append(endRow + 1);
      }

      namedRange.RefersToFormula = forumla.ToString();
    }

    public void CreateRange(string name, int startCell, int startRow, int endCell, int endRow)
    {
      if (startRow > endRow)
      {
        throw new InvalidOperationException("CreateRange - startRow can not be greater to endRow");
      }

      var namedRange = GetName(name);
      var forumla = new StringBuilder();
      forumla.Append(Name)
        .Append("!$")
        .Append(CellReference.ConvertNumToColString(startCell))
        .Append("$")
        .Append(startRow + 1)
        .Append(":$")
        .Append(CellReference.ConvertNumToColString(endCell))
        .Append("$")
        .Append(endRow + 1);
      namedRange.RefersToFormula = forumla.ToString();
    }

    public void CreateHierarchicalRange(string mainName, string tableName, string newName, int rowIndex, int cellIndex,
      string sheetName)
    {
      var namedRange = GetName(newName);
      var forumla = new StringBuilder();
      forumla.Append("INDEX(")
        .Append(tableName)
        .Append(", MATCH(")
        .Append(sheetName)
        .Append("!XFD1")
        .Append(",")
        .Append(mainName)
        .Append(",0),0)");
      namedRange.RefersToFormula = forumla.ToString();
    }

    public void MergeCells(int startRow, int endRow, int startColumn, int endColumn)
    {
      _sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));
    }

    public IStyle CreateStyle(int column, int row)
    {
      var rowObj = _sheet.GetRow(row) ?? _sheet.CreateRow(row);
      var cell = rowObj.GetCell(column) ?? rowObj.CreateCell(column);
      var style = ExcelFile.Excel.CreateCellStyle();
      style.CloneStyleFrom(cell.CellStyle);
      return new NPoiStyle(style, ExcelFile.Excel);
    }

    public IStyle CreateStyle()
    {
      var style = ExcelFile.Excel.CreateCellStyle();
      return new NPoiStyle(style, ExcelFile.Excel);
    }

    public bool Visible
    {
      get
      {
        var index = ExcelFile.Excel.GetSheetIndex(_sheet);
        return !ExcelFile.Excel.IsSheetHidden(index) && !vbaSheetNames.Any(n => Name.StartsWith(n));
      }
    }

    private IName GetName(string name)
    {
      var index = ExcelFile.Workbook.GetNameIndex(name);
      if (index != -1)
      {
        return ExcelFile.Workbook.GetNameAt(index);
      }

      var namedRange = ExcelFile.Workbook.CreateName();
      namedRange.NameName = name;
      return namedRange;
    }
  }
}