using System;

namespace ViennaNET.WebApi.Validation
{
  public sealed class HostBuilderValidationException : Exception
  {
    public HostBuilderValidationException(string message) : base(message)
    {
    }
  }
}
