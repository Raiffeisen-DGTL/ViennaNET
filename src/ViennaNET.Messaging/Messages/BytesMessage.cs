using System;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Messages
{
  /// <summary>
  ///   Сообщение
  /// </summary>
  [Serializable]
  public class BytesMessage : BaseMessage
  {
    /// <summary>
    ///   Данные сообщения
    /// </summary>
    [NotNull]
    public byte[] Body { get; set; }

    /// <inheritdoc />
    public override string LogBody()
    {
      return IsEmpty()
        ? "empty bytes buffer"
        : $"bytes buffer with length = {Body.Length}";
    }

    /// <inheritdoc cref="BaseMessage"/>
    public override bool IsEmpty()
    {
      return Body == null || Body.Length == 0;
    }
  }
}