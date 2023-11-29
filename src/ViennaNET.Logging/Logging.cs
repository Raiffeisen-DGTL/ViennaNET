using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ViennaNET.Logging
{
  /// <summary>
  /// Service operations for logging
  /// </summary>
  [ExcludeFromCodeCoverage]
  public sealed partial class Logging
  {
    /// <summary>
    /// Correct "no data" in logs on executed assembly namespace
    /// </summary>
    public static void CorrectServiceName()
    {
      var rootAssembly = Assembly.GetEntryAssembly()
                         ?? Assembly.GetCallingAssembly();
      var serviceName = rootAssembly.GetName().Name;

      CorrectServiceName(serviceName);
    }

    /// <summary>
    /// Correct "no data" in logs
    /// </summary>
    /// <param name="value">service value</param>
    public static void CorrectServiceName(string value)
    {
      Logger.DefaultService = value;
    }
  }
}