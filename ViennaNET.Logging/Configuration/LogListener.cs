using System;
using System.Collections.Generic;

namespace ViennaNET.Logging.Configuration
{
  /// <summary>
  /// class for storing single log listener configuration
  /// </summary>
  [Serializable]
  public class LogListener
  {
    /// <summary>
    /// C'tor
    /// </summary>
    public LogListener()
    {
      Params = new Dictionary<string, string>();
      Type = TypeDefault;
      MinLevel = MinLevelDefault;
      MaxLevel = MaxLevelDefault;
      Category = CategoryDefault;
    }

    /// <summary>
    /// listener type
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// min logging level
    /// </summary>
    public LogLevel MinLevel { get; set; }

    /// <summary>
    /// max logging level
    /// </summary>
    public LogLevel MaxLevel { get; set; }

    /// <summary>
    /// listen category (string.Empty is All categories by default
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// specific for the concrete log listener type parameters in form of key->value
    /// </summary>
    public Dictionary<string, string> Params { get; set; }

    #region default constant values

    /// <summary>
    /// default value of the log listener none 
    /// </summary>
    private const string TypeDefault = "None";

    /// <summary>
    /// minimum default log level 
    /// </summary>
    private const LogLevel MinLevelDefault = LogLevel.Debug;

    /// <summary>
    /// maximum default log level
    /// </summary>
    private const LogLevel MaxLevelDefault = LogLevel.Error;

    /// <summary>
    /// default log level category (empty string - All by default)
    /// </summary>
    private const string CategoryDefault = "";

    /// <summary>
    /// public constant for all categopry
    /// </summary>
    public static readonly string CategoryAll = string.Empty;

    #endregion
  }
}