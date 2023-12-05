using System;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;

namespace ViennaNET.Excel.Impl
{
  internal static class CellErrorProcessor
  {
    private static bool IsExceptionAccepted(Exception exception, out ErrorType type)
    {
      type = ErrorType.WrongFormula;
      if (exception is RuntimeException)
      {
        if (exception.Message.Contains("Workbook environment has not been set up"))
        {
          type = ErrorType.BadReference;
          return true;
        }

        return exception.Message.Contains("Could not resolve external workbook name")
               || exception.Message.Contains("Unexpected ptg class (ArrayPtg)");
      }

      return exception is NotImplementedException;
    }

    private static string FormatException(ICell cell, Exception ex, ErrorType type)
    {
      var message = string.Empty;
      if (type == ErrorType.WrongFormula)
      {
        message =
          $"В ячейке {CellReference.ConvertNumToColString(cell.ColumnIndex)}{cell.RowIndex + 1} обнаружена недопустимая формула {cell.CellFormula ?? string.Empty}";
      }
      else if (type == ErrorType.BadReference)
      {
        message = $"В файле содержится ссылка на внешний файл: {ex.Message}";
      }

      return message;
    }

    private static void ProcessException(Exception ex, string message)
    {
      throw new ExcelProcessingException(ex, "{0}", message);
    }

    public static bool Handled(ICell cell, Exception exception)
    {
      if (IsExceptionAccepted(exception, out var type))
      {
        ProcessException(exception, FormatException(cell, exception, type));
      }

      return false;
    }

    private enum ErrorType
    {
      WrongFormula,
      BadReference
    }
  }
}