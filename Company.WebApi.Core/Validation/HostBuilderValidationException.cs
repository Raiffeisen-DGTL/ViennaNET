using System;

namespace Company.WebApi.Core.Validation
{
  public sealed class HostBuilderValidationException : Exception
  {
    public HostBuilderValidationException(string message) : base(message)
    {
    }
  }
}
