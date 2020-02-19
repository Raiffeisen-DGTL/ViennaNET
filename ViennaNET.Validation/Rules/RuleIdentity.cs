using System;

namespace ViennaNET.Validation.Rules
{
  /// <summary>
  /// Идентификатор правила
  /// </summary>
  public sealed class RuleIdentity
  {
    /// <summary>
    /// Инициализирует идентификатор правила уникальной строкой
    /// </summary>
    /// <param name="code">Код правила</param>
    public RuleIdentity(string code)
    {
      if (string.IsNullOrEmpty(code))
      {
        throw new ArgumentException(code);
      }

      Code = code;
    }
    
    /// <summary>
    /// Код правила
    /// </summary>
    public string Code { get; }

    private bool Equals(RuleIdentity other)
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

      return obj is RuleIdentity identity && Equals(identity);
    }

    public override int GetHashCode()
    {
      return Code != null
        ? Code.GetHashCode()
        : 0;
    }

    public static bool operator ==(RuleIdentity left, RuleIdentity right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(RuleIdentity left, RuleIdentity right)
    {
      return !Equals(left, right);
    }
  }
}
