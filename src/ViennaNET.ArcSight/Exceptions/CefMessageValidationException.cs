using System;

namespace ViennaNET.ArcSight.Exceptions
{
  /// <summary>
  ///   Исключение, возникающее при наличии ошибки в
  ///   значениях полей передаваемого сообщения. <see cref="CefMessage" />
  /// </summary>
  public class CefMessageValidationException : Exception
  {
    public CefMessageValidationException(string message) : base(message)
    {
    }
  }
}