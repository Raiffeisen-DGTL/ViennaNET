using System.IO;

namespace ViennaNET.Excel.Contracts
{
  public interface IExcelProcessor
  {
    void AddConditionalFormatting(string fileName);

    void RemoveInvalidUrls(Stream stream);
  }
}