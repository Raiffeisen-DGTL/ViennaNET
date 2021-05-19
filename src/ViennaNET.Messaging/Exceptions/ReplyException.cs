using System;

namespace ViennaNET.Messaging.Exceptions
{
  /// <summary>
  /// Ошибка во время ответа хендлером
  /// </summary>
  public class ReplyException : MessagingException
  {
    /// <inheritdoc />
    public ReplyException(Exception innerException, string message) : base(innerException, message)
    {
    }
  }
}
