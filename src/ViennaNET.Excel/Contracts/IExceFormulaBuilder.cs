namespace ViennaNET.Excel.Contracts
{
  public interface IExceFormulaBuilder
  {
    string CreateFormula(string function, int startRow, int startColumn, int endRow, int endColumn);

    string CreateCount(int startRow, int startColumn, int endRow, int endColumn);

    string CreateSumWithRound(int startRow, int startColumn, int endRow, int endColumn);

    string CreateRoundWithMultiply2Cells(int row, int firstCell, int secondCell);

    string CreateRoundWithIfAndMultiply2Cells(int row, int firstCell, int secondCell, int thirdCell);

    string CreateNamedRangeSelectionWithErrorString(string rowName, string tableName, int columnIndex,
      string defaultText);
  }
}