using System;
using System.Collections;
using System.Linq;
using ViennaNET.Excel.Contracts;
using NPOI.SS.UserModel;

namespace ViennaNET.Excel.Impl
{
  internal class NPoiRow : IExcelRow
  {
    private readonly IRow _row;
    private readonly NPoiWorkSheet _sheet;

    internal NPoiRow(IRow row, NPoiWorkSheet sheet)
    {
      _row = row ?? throw new ArgumentNullException(nameof(row));
      _sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
    }

    public int CellCount
    {
      get { return _row.Cells.Count; }
    }

    public object this[int index]
    {
      get
      {
        var cell = _row.GetCell(index);
        return GetCell(cell);
      }
      set
      {
        var cell = _row.GetCell(index) ?? _row.CreateCell(index);
        SetCell(cell, value);
      }
    }

    public void SetFormula(int index, string formula)
    {
      var cell = _row.GetCell(index) ?? _row.CreateCell(index);
      cell.SetCellFormula(formula);
    }

    public void SetStyle(int index, IStyle style)
    {
      var cell = _row.GetCell(index) ?? _row.CreateCell(index);
      var npoiStyle = style as NPoiStyle;
      if (npoiStyle != null)
      {
        cell.CellStyle = npoiStyle.Style;
      }
    }

    public void ProtectCell(int index)
    {
      var cell = _row.GetCell(index) ?? _row.CreateCell(index);
      cell.CellStyle.IsLocked = true;
    }

    public void UnProtectCell(int index)
    {
      var cell = _row.GetCell(index) ?? _row.CreateCell(index);
      cell.CellStyle.IsLocked = false;
    }

    public IEnumerable Cells()
    {
      return _row.Cells.Select(GetCell);
    }

    public bool Empty => _row.Cells.Count == 0;

    public bool IsBlank => _row.Cells.All(x => x.CellType == CellType.Blank);

    public int RowNum => _row.RowNum;

    public bool IsHidden => _row.ZeroHeight;

    public void HideCell(int index)
    {
      var style = _sheet.ExcelFile.Workbook.CreateCellStyle();
      var cell = _row.GetCell(index);
      style.CloneStyleFrom(cell.CellStyle);
      style.DataFormat = _sheet.ExcelFile.HiddenDataFormat;
      cell.CellStyle = style;
    }

    public void SetTextFormat(int cellIndex)
    {
      var cell = _row.GetCell(cellIndex) ?? _row.CreateCell(cellIndex);
      var creationHelper = _sheet.ExcelFile.Workbook.GetCreationHelper();
      var format = creationHelper.CreateDataFormat()
                                 .GetFormat("TEXT");
      cell.CellStyle.DataFormat = format;
    }

    private static bool IsDateValue(CellValue cellValue, ICell cell)
    {
      if (!DateUtil.IsValidExcelDate(cellValue.NumberValue))
      {
        return false;
      }

      var cellStyle = cell.CellStyle;
      return cellStyle != null && DateUtil.IsADateFormat(cellStyle.DataFormat, cellStyle.GetDataFormatString());
    }

    private object TryToEvaluateFormula(ICell cell)
    {
      var evaluator = _sheet.ExcelFile.Excel.GetCreationHelper()
                            .CreateFormulaEvaluator();
      CellValue cellValue = null;
      try
      {
        cellValue = evaluator.Evaluate(cell);
      }
      catch (Exception ex)
      {
        if (!CellErrorProcessor.Handled(cell, ex))
        {
          throw;
        }
      }
      switch (cellValue.CellType)
      {
        case CellType.Boolean:
          return cellValue.BooleanValue;
        case CellType.Error:
        case CellType.Formula:
          return cell.CellFormula;
        case CellType.Numeric:
          if (IsDateValue(cellValue, cell))
          {
            return DateUtil.GetJavaDate(cellValue.NumberValue);
          }
          return cellValue.NumberValue;
        case CellType.String:
          return cellValue.StringValue;
        default:
          return null;
      }
    }

    private object GetCell(ICell cell)
    {
      if (cell == null)
      {
        return null;
      }

      switch (cell.CellType)
      {
        case CellType.Blank:
          return null;
        case CellType.Boolean:
          return cell.BooleanCellValue;
        case CellType.Error:
          return null;
        case CellType.Formula:
          return TryToEvaluateFormula(cell);
        case CellType.Numeric:
          if (DateUtil.IsCellDateFormatted(cell))
          {
            return DateUtil.GetJavaDate(cell.NumericCellValue);
          }
          return cell.NumericCellValue;
        case CellType.String:
          return cell.StringCellValue;
        default:
          throw new InvalidOperationException(string.Format("Can not find value for cell {0}, row {1}", cell.ColumnIndex, _row.RowNum));
      }
    }

    public double GetNumericCellValue(int cellIndex)
    {
      return _row.Cells[cellIndex].NumericCellValue;
    }

    private static void SetCell(ICell cell, object value)
    {
      if (value == null)
      {
        cell.SetCellValue(string.Empty);
      }
      else
      {
        var type = value.GetType();
        if (type == typeof(short) || type == typeof(byte) || type == typeof(int) || type == typeof(long) || type == typeof(float)
            || type == typeof(double))
        {
          cell.SetCellValue(Convert.ToDouble(value));
        }
        else if (type == typeof(bool))
        {
          cell.SetCellValue(Convert.ToBoolean(value));
        }
        else if (type == typeof(DateTime))
        {
          cell.SetCellValue(Convert.ToDateTime(value));
        }
        else if (type == typeof(string))
        {
          cell.SetCellValue(Convert.ToString(value));
        }
        else if (type.GetInterfaces().Contains(typeof(IHyperlink)))
        {
          var link = (IHyperlink)value;
          cell.SetCellValue(Convert.ToString(link.Label ?? link.Address));
          cell.Hyperlink = link;
        }
        else
        {
          throw new InvalidOperationException($"Unsupported type '{type}'");
        }
      }
    }
  }
}