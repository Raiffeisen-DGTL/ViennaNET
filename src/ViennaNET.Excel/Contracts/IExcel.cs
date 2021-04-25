#region usings

using System.Collections.Generic;
using System.IO;

#endregion

namespace ViennaNET.Excel.Contracts
{
  /// <summary>
  /// Interfaces for work with Excel files
  /// </summary>
  public interface IExcel
  {
    /// <summary>
    /// Create sheet of Excel file
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IWorkSheet Create(string name = "");

    /// <summary>
    /// Get sheet by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IWorkSheet this[string name] { get; }

    /// <summary>
    /// Get shit by index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IWorkSheet this[int index] { get; }

    /// <summary>
    /// Get count of worksheets
    /// </summary>
    int WorkSheetCount { get; }

    /// <summary>
    /// Get file name
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// Get sheets of file
    /// </summary>
    /// <returns></returns>
    IEnumerable<IWorkSheet> WorkSheets();

    /// <summary>
    /// Save excel file
    /// </summary>
    void Save();

    /// <summary>
    /// Save file to stream
    /// </summary>
    /// <param name="stream"></param>
    void Save(Stream stream);

    /// <summary>
    /// Get file stream in byte array
    /// </summary>
    /// <returns></returns>
    byte[] ToByteArray();

    /// <summary>
    /// Open excel file
    /// </summary>
    void Open();
  }
}
