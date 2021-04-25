using System;

namespace ViennaNET.Validation.Rules.ValidationResults.RuleMessages
{
  /// <summary>
  /// Идентификатор сообщения
  /// </summary>
  public sealed class MessageIdentity
  {
    /// <summary>
    /// Инициализирует идентификатор сообщения уникальной строкой
    /// </summary>
    /// <param name="code">Код сообщения</param>
    public MessageIdentity(string code)
    {
      if (string.IsNullOrEmpty(code))
      {
        throw new ArgumentException(code);
      }

      Code = code;
    }

    /// <summary>
    /// Код сообщения
    /// </summary>
    public string Code { get; }

    private bool Equals(MessageIdentity other)
    {
      return string.Equals(Code, other.Code);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }

      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      return obj is MessageIdentity identity && Equals(identity);
    }

    public override int GetHashCode()
    {
      return Code != null
        ? Code.GetHashCode()
        : 0;
    }

    public static bool operator ==(MessageIdentity left, MessageIdentity right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(MessageIdentity left, MessageIdentity right)
    {
      return !Equals(left, right);
    }
  }
}
