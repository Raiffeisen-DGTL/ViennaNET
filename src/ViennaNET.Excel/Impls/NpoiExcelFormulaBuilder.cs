using System.Globalization;
using System.Text;
using NPOI.SS.Util;
using ViennaNET.Excel.Contracts;

namespace ViennaNET.Excel.Impl
{
  public class NpoiExcelFormulaBuilder : IExceFormulaBuilder
  {
    private const string count = "COUNT";
    private const string referenceDelimiter = ":";
    private const string squareBegin = "(";
    private const string squareEnd = ")";
    private const string sumWithRoundBegin = "INT(SUM(";
    private const string sumWithRoundEnd = ")*100+0.5)/100";

    private const string namedRangeFormula = "IF(ISERROR(VLOOKUP({0},{1},{2},FALSE)),\"{3}\",VLOOKUP({0},{1},{2},FALSE))";

    private const string roundMultiply = "ROUND( {0} * {1} / 100, 2)";
    private const string roundIfMultiply = "IF({0} < ROUND({0} * {2} / 100, 2) + {1}, {0} - {1}, ROUND({0} * {2} / 100, 2))";

    public string CreateFormula(string function, int startRow, int startColumn, int endRow, int endColumn)
    {
      var stream = new StringBuilder();
      stream.Append(function.ToUpper());
      stream.Append(squareBegin);
      stream.Append(CellReference.ConvertNumToColString(startColumn));
      stream.Append(startRow + 1);
      stream.Append(referenceDelimiter);
      stream.Append(CellReference.ConvertNumToColString(endColumn));
      stream.Append(endRow + 1);
      stream.Append(squareEnd);
      return stream.ToString();
    }

    public string CreateCount(int startRow, int startColumn, int endRow, int endColumn)
    {
      var countFormula = CreateFormula(count, startRow, startColumn, endRow, endColumn);
      return countFormula;
    }

    public string CreateSumWithRound(int startRow, int startColumn, int endRow, int endColumn)
    {
      var stream = new StringBuilder();
      stream.Append(sumWithRoundBegin);
      stream.Append(CellReference.ConvertNumToColString(startColumn));
      stream.Append(startRow + 1);
      stream.Append(referenceDelimiter);
      stream.Append(CellReference.ConvertNumToColString(endColumn));
      stream.Append(endRow + 1);
      stream.Append(sumWithRoundEnd);
      return stream.ToString();
    }

    public string CreateRoundWithMultiply2Cells(int rowNum, int firstCellNum, int secondCellNum)
    {
      var firstCell = CellReference.ConvertNumToColString(firstCellNum) + (rowNum + 1).ToString(CultureInfo.InvariantCulture);
      var secondCell = CellReference.ConvertNumToColString(secondCellNum) + (rowNum + 1).ToString(CultureInfo.InvariantCulture);
      return string.Format(roundMultiply, firstCell, secondCell);
    }

    public string CreateRoundWithIfAndMultiply2Cells(int rowNum, int firstCellNum, int secondCellNum, int thirdCellNum)
    {
      var firstCell = CellReference.ConvertNumToColString(firstCellNum) + (rowNum + 1).ToString(CultureInfo.InvariantCulture);
      var secondCell = CellReference.ConvertNumToColString(secondCellNum) + (rowNum + 1).ToString(CultureInfo.InvariantCulture);
      var thirdCell = CellReference.ConvertNumToColString(thirdCellNum) + (rowNum + 1).ToString(CultureInfo.InvariantCulture);
      return string.Format(roundIfMultiply, firstCell, secondCell, thirdCell);
    }

    public string CreateNamedRangeSelectionWithErrorString(string rowName, string tableName, int columnIndex, string defaultText)
    {
      return string.Format(namedRangeFormula, rowName, tableName, columnIndex, defaultText.Replace("\"", "\'"));
    }
  }
}
