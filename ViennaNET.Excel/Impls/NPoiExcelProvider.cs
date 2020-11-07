using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ViennaNET.Excel.Contracts;
using ViennaNET.FileUtils.Interfaces;

namespace ViennaNET.Excel.Impl
{
  public class NPoiExcelProvider : IExcelProvider
  {
    private static readonly string[] validExtentions = { ".xls", ".xlsx", ".xlsm" };

    private static readonly byte[] zipArchiveMagicNumber = { 0x50, 0x4B, 0x03, 0x04 };
    private static readonly byte[] xlsMagicNumber = { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
    private readonly IFileUtils _fileUtils;
    private readonly IExcelProcessor _processor;

    static NPoiExcelProvider()
    {
      NPoiFunctionEval.ReplaceFunction("round", new NPoiRound());
    }

    public NPoiExcelProvider(IFileUtils fileUtils, IExcelProcessor processor)
    {
      _fileUtils = fileUtils ?? throw new ArgumentNullException(nameof(fileUtils));
      _processor = processor ?? throw new ArgumentNullException(nameof(processor));
    }

    public bool IsXml(string fileName, Stream fileContent)
    {
      if (string.IsNullOrEmpty(fileName))
      {
        return false;
      }

      var extention = Path.GetExtension(fileName);

      if (!validExtentions.Any(ext => ext.Equals(extention, StringComparison.InvariantCultureIgnoreCase)))
      {
        return false;
      }

      try
      {
        if (fileContent == null)
        {
          throw new ArgumentNullException(nameof(fileContent));
        }

        var fileHeader = new byte[8];
        if (fileContent.Read(fileHeader, 0, 8) == 8)
        {
          if (FileStartsWithMagicNumber(fileHeader, zipArchiveMagicNumber))
          {
            return true;
          }

          if (FileStartsWithMagicNumber(fileHeader, xlsMagicNumber))
          {
            return false;
          }
        }
      }
      finally
      {
        fileContent?.Seek(0, SeekOrigin.Begin);
      }

      return false;
    }

    public IExcel Read(string fileName)
    {
      using (var file = _fileUtils.GetStreamFromFile(fileName))
      {
        CheckFileName(fileName ?? throw new ArgumentNullException(nameof(fileName)));

        var isXml = IsXml(fileName, file);
        file.Seek(0, SeekOrigin.Begin);

        if (isXml)
        {
          try
          {
            _processor.RemoveInvalidUrls(file);
          }
          catch
          {
            // ignored
            // do nothing in case of password protected file
          }
          file.Seek(0, SeekOrigin.Begin);
        }

        return new NPoiExcelFile(file, _fileUtils, isXml, fileName);
      }
    }

    public IExcel Create(string fileName, bool isXml)
    {
      using (var file = _fileUtils.GetStreamFromFile(fileName))
      {
        return new NPoiExcelFile(file, fileName, isXml);
      }
    }

    /// <summary>
    /// Читаем либо создаем новый документ
    /// </summary>
    /// <param name="stream">File Stream</param>
    /// <param name="isXml"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public IExcel Read(Stream stream, bool isXml, string fileName = "")
    {
      if (!string.IsNullOrEmpty(fileName))
      {
        CheckFileName(fileName);
        if (!isXml)
        {
          return new NPoiExcelFile(stream, _fileUtils, false, fileName);
        }
        try
        {
          _processor.RemoveInvalidUrls(stream);
        }
        catch
        {
          // ignored
          // do nothing in case of password protected file
        }
        stream.Seek(0, SeekOrigin.Begin);
      }
      return new NPoiExcelFile(stream, _fileUtils, isXml, fileName);
    }

    public void Write(IExcel excel, string fileName)
    {
      if (File.Exists(fileName))
      {
        File.Delete(fileName);
      }
      using (var stream = File.OpenWrite(fileName))
      {
        ((NPoiExcelFile)excel).Excel.Write(stream);
      }
    }

    private static bool FileStartsWithMagicNumber(byte[] fileContent, IEnumerable<byte> magicNumber)
    {
      if (fileContent == null)
      {
        return false;
      }

      return !magicNumber.Where((t, i) => fileContent[i] != t)
                         .Any();
    }

    private static void CheckFileName(string fileName)
    {
      var extension = Path.GetExtension(fileName);
      if (string.IsNullOrEmpty(extension)
          || !validExtentions.Any(ext => ext.Equals(extension, StringComparison.InvariantCultureIgnoreCase)))
      {
        throw new BadExcelFormatExeption("Неверный формат файла");
      }
    }
  }
}
