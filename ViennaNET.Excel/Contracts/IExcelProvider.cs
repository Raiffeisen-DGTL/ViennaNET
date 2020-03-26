using System.IO;

namespace ViennaNET.Excel.Contracts
{
  public interface IExcelProvider
  {
    IExcel Create(string fileName, bool isXml);

    bool IsXml(string fileName, Stream fileContent);

    IExcel Read(string fileName);

    IExcel Read(Stream stream, bool isXml, string fileName = "");

    void Write(IExcel excel, string fileName);
  }
}
