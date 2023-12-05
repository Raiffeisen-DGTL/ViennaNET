﻿using System;
using System.Collections.Generic;
using System.Linq;
using ViennaNET.Logging.Contracts;

namespace ViennaNET.Logging.Configuration
{
  /// <summary>
  ///   logger configuration
  /// </summary>
  [Serializable]
  public class LoggerConfiguration
  {
    #region default values

    private const bool EnabledDefault = true;

    #endregion

    public LoggerConfiguration()
    {
      Enabled = EnabledDefault;
      Listeners = new List<LogListener>();
      Types = new List<ListenerTypeDescription>();
    }

    public bool IsFromConfig { get; set; }

    public bool Enabled { get; set; }
    public List<LogListener> Listeners { get; private set; }

    public List<ListenerTypeDescription> Types { get; private set; }

    public IListener GetCustomListener(string name)
    {
      var customTypeDescr = Types.FirstOrDefault(t => t.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
      if (customTypeDescr != null)
      {
        return (IListener)Activator.CreateInstance(customTypeDescr.Type);
      }

      return null;
    }
  }
}