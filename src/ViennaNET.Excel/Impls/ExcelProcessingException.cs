using System;
using System.Runtime.Serialization;

namespace ViennaNET.Excel.Impl
{
  [Serializable]
  public class ExcelProcessingException : Exception
  {
    public ExcelProcessingException()
    {
    }

    public ExcelProcessingException(string message, params object[] args) : base(string.Format(message, args))
    {
    }

    public ExcelProcessingException(Exception innerException, string message, params object[] args) : base(
      string.Format(message, args), innerException)
    {
    }

    public ExcelProcessingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}