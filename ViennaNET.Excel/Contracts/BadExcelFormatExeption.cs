using System;
using System.Runtime.Serialization;

namespace ViennaNET.Excel.Contracts
{
  public class BadExcelFormatExeption : Exception
  {
    public BadExcelFormatExeption()
    {
    }

    public BadExcelFormatExeption(string message, params object[] args) : base(string.Format(message, args))
    {
    }

    public BadExcelFormatExeption(Exception innerException, string message, params object[] args) : base(string.Format(message, args), innerException)
    {
    }

    public BadExcelFormatExeption(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ExcelErrorType Type { get; set; }

    public string FileName { get; set; }

    public bool IsXmlFormat { get; set; }
  }
}
