using System.Collections;

namespace ViennaNET.Excel.Contracts
{
  public interface IExcelRow
  {
    int CellCount { get; }

    object this[int index] { get; set; }

    bool Empty { get; }

    bool IsBlank { get; }

    int RowNum { get; }

    bool IsHidden { get; }

    void SetFormula(int index, string formula);

    void SetStyle(int index, IStyle style);

    void ProtectCell(int index);

    void UnProtectCell(int index);

    IEnumerable Cells();

    void HideCell(int index);

    void SetTextFormat(int cellIndex);

    double GetNumericCellValue(int cellIndex);
  }
}