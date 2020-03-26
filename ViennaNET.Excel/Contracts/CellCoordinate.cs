namespace ViennaNET.Excel.Contracts
{
  public class CellCoordinate
  {
    public CellCoordinate()
    {
    }

    public CellCoordinate(int row, int cell)
    {
      CellIndex = cell;
      RowIndex = row;
    }

    public int CellIndex { get; set; }
    public int RowIndex { get; set; }
  }
}
