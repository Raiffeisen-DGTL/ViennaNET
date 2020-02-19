using System;
using ViennaNET.Utils;

namespace ViennaNET.Validation.Tests.RuleValidation.When
{
  public sealed class WhenException : Exception
  {
    public WhenException([NotNull] string message, [CanBeNull] params object[] args) : base(string.Format(message, args))
    {
    }

    public WhenException([NotNull] Exception innerException, [NotNull] string message, [CanBeNull] params object[] args) :
      base(string.Format(message, args), innerException)
    {
    }
  }
}
