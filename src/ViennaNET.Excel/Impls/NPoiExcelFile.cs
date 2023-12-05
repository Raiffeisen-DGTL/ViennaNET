using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using NPOI;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using ViennaNET.Excel.Contracts;
using ViennaNET.FileUtils.Interfaces;

namespace ViennaNET.Excel.Impl
{
  internal class NPoiExcelFile : IExcel
  {
    private const string xlsxInXlsMessage =
      "The supplied data appears to be in the Office 2007+ XML. You are calling the part of POI that deals with OLE2 Office Documents. You need to call a different part of POI to process this data (eg XSSF instead of HSSF)";

    private const string wrongPasswordMessage = "Default password is invalid for docId/saltData/saltHash";

    private const string passwordRequiredMessage =
      "The supplied POIFSFileSystem does not contain a BIFF8 'Workbook' entry. Is it really an excel file?";

    private const string oldFormat =
      "The supplied spreadsheet seems to be Excel 5.0/7.0 (BIFF5) format. POI only supports BIFF8 format (from Excel versions 97/2000/XP/2003)";

    private const string badFormat = "Unexpected missing row when some rows already present, the file is wrong";
    private const string badRecordFormat = "Unable to construct record instance";
    private const string inavlidHeaderSignature = "Invalid header signature";
    private const string keyNotFound = "The given key was not present in the dictionary.";
    private const string wrongLocalHeader = "Wrong Local header signature: ";
    private readonly IFileUtils _utils;
    internal readonly IWorkbook Excel;

    internal NPoiExcelFile(Stream sw, string fileName, bool isXml)
    {
      var stream = sw ?? throw new ArgumentNullException();

      try
      {
        if (isXml)
        {
          Excel = new XSSFWorkbook();
        }
        else
        {
          Excel = new HSSFWorkbook();
        }

        Excel.Write(stream);
      }
      catch (Exception exception)
      {
        ProcessException(exception, fileName, isXml);
      }
    }

    internal NPoiExcelFile(Stream stream, IFileUtils utils, bool isXml, string fileName = "")
    {
      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }

      FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));

      _utils = utils ?? throw new ArgumentNullException(nameof(utils));
      try
      {
        if (isXml)
        {
          Excel = new XSSFWorkbook(stream);
        }
        else
        {
          Excel = new HSSFWorkbook(stream);
        }
      }
      catch (Exception exception)
      {
        ProcessException(exception, fileName, isXml);
      }

      HiddenDataFormat = Excel.GetCreationHelper()
        .CreateDataFormat()
        .GetFormat(";;;");
    }

    /// <summary>
    ///   Возвращает книгу
    /// </summary>
    public IWorkbook Workbook => Excel;

    public short HiddenDataFormat { get; }

    /// <summary>
    ///   Создает новый лист и возвращает его
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IWorkSheet Create(string name = "")
    {
      return new NPoiWorkSheet(Excel.CreateSheet(name), this);
    }

    /// <summary>
    ///   Возвращает лист Excel по имени.
    /// </summary>
    public IWorkSheet this[string name]
    {
      get
      {
        var sheet = Excel.GetSheet(name);
        if (sheet == null)
        {
          return null;
        }

        return new NPoiWorkSheet(Excel.GetSheet(name), this);
      }
    }

    /// <summary>
    ///   Вохвращает рабочий лист по индексу
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IWorkSheet this[int index]
    {
      get
      {
        try
        {
          var sheet = Excel.GetSheetAt(index);
          if (sheet == null)
          {
            return null;
          }

          return new NPoiWorkSheet(sheet, this);
        }
        catch (ArgumentOutOfRangeException)
        {
          return null;
        }
      }
    }

    public int WorkSheetCount => Excel.NumberOfSheets;

    public string FileName { get; }

    public void Save()
    {
      if (string.IsNullOrEmpty(FileName))
      {
        throw new InvalidOperationException(
          "Can not save excel file - file name is empty. Use Excel provider to specify file name");
      }

      using (var stream = new FileStream(FileName, FileMode.Create))
      {
        Excel.Write(stream);
      }
    }

    public void Save(Stream stream)
    {
      if (stream != null)
      {
        Excel.Write(stream);
      }
      else
      {
        throw new ArgumentNullException("Stream for file saving is null");
      }
    }

    public byte[] ToByteArray()
    {
      byte[] bytes;
      using (var stream = new MemoryStream())
      {
        Excel.Write(stream);
        bytes = stream.ToArray();
      }

      return bytes;
    }

    public void Open()
    {
      Save();
      Process.Start(FileName);
      //_utils.OpenInWindow(FileName);
    }

    public IEnumerable<IWorkSheet> WorkSheets()
    {
      for (var i = 0; i < Excel.NumberOfSheets; i++)
      {
        yield return new NPoiWorkSheet(Excel.GetSheetAt(i), this);
      }
    }

    private static bool IsMatch<T>(Exception exception, string message) where T : Exception
    {
      var typedException = exception as T;
      return typedException?.Message.Equals(message) ?? false;
    }

    private static bool IsMatch<T>(Exception exception) where T : Exception
    {
      return exception as T != null;
    }

    private static bool IsContains<T>(Exception exception, string message) where T : Exception
    {
      var typedException = exception as T;
      return typedException?.Message.Contains(message) ?? false;
    }

    private static void ProcessException(Exception exception, string fileName, bool isXml)
    {
      var message = "Неверный формат файла";
      var type = ExcelErrorType.Unknown;
      if (IsMatch<UriFormatException>(exception))
      {
        message = "Ошибка при открытии файла: неорректно задана ссылка или e-mail в файле";
        type = ExcelErrorType.BadUrl;
      }
      else if (IsMatch<OfficeXmlFileException>(exception, xlsxInXlsMessage))
      {
        type = ExcelErrorType.XlsxAsXls;
      }
      else if (IsMatch<EncryptedDocumentException>(exception, wrongPasswordMessage))
      {
        type = ExcelErrorType.PasswordProtected;
      }
      else if (IsMatch<OldExcelFormatException>(exception, oldFormat))
      {
        type = ExcelErrorType.OldFormat;
      }
      else if (IsMatch<Exception>(exception, badFormat))
      {
        type = ExcelErrorType.BadFormat;
      }
      else if (IsMatch<RecordFormatException>(exception, badRecordFormat))
      {
        if (exception.InnerException != null &&
            IsContains<RecordFormatException>(exception.InnerException, "Unknown encryption info"))
        {
          type = ExcelErrorType.PasswordProtected;
        }
        else
        {
          type = ExcelErrorType.BadFormat;
        }
      }
      else if (IsContains<IOException>(exception, inavlidHeaderSignature))
      {
        type = ExcelErrorType.BadFormat;
      }
      else if (IsMatch<ArgumentException>(exception, passwordRequiredMessage))
      {
        type = ExcelErrorType.PasswordProtected;
      }
      else if (IsMatch<KeyNotFoundException>(exception, keyNotFound))
      {
        type = ExcelErrorType.BadFormat;
      }
      else if (IsContains<ZipException>(exception, wrongLocalHeader))
      {
        type = ExcelErrorType.PasswordProtected;
      }

      throw new BadExcelFormatExeption(exception, message) { FileName = fileName, IsXmlFormat = isXml, Type = type };
    }
  }
}