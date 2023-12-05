using System.Collections.Generic;

namespace ViennaNET.Excel.Contracts
{
  public interface IWorkSheet
  {
    string Name { get; }

    IExcelRow this[int i] { get; }

    bool Visible { get; }

    IEnumerable<IExcelRow> Rows();

    void HideColumn(int column);

    void ShowColumn(int column);

    bool ColumnHidden(int column);

    int GetColumnWidth(int column);

    void CopyStyles(int fromRow, int fromColumn, int startRow, int startColumn, int endRow, int endColumn,
      string dataFormat = null);

    void SetColumnWidth(int column, int width);

    void ShiftDownRow(int row, int rowNumber = 1);

    void RecalculateFormulas();

    void HideRow(int row);

    object ReadCellByName(string name);

    void CreateVerticalRange(string name, int cell, int startRow, int endRow);

    void CreateRange(string name, int startCell, int startRow, int endCell, int endRow);

    void CreateHierarchicalRange(string mainName, string tableName, string newName, int rowIndex, int cellIndex,
      string sheetName);

    void MergeCells(int startRow, int endRow, int startColumn, int endColumn);

    IStyle CreateStyle(int columns, int row);

    IStyle CreateStyle();
  }
}