namespace ViennaNET.Excel.Contracts
{
  public static class ExcelExtentions
  {
    public static void SetValue(this IWorkSheet sheet, CellCoordinate coordinate, object value)
    {
      bool canAssign;
      if (value is string str)
      {
        canAssign = !string.IsNullOrEmpty(str);
      }
      else
      {
        canAssign = value != null;
      }

      if (canAssign)
      {
        sheet[coordinate.RowIndex][coordinate.CellIndex] = value;
      }
    }

    public static void AppendValue(this IWorkSheet sheet, CellCoordinate coordinate, object value)
    {
      var cellVal = sheet[coordinate.RowIndex][coordinate.CellIndex];
      var str = cellVal == null
        ? string.Empty
        : cellVal.ToString();
      if (string.IsNullOrEmpty(str))
      {
        str = " ";
      }

      sheet[coordinate.RowIndex][coordinate.CellIndex] = str + value;
    }
  }
}