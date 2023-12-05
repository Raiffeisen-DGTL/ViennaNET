﻿#region usings

using System;

#endregion

namespace ViennaNET.Logging.Configuration
{
  /// <summary>
  ///   listener type description
  /// </summary>
  public class ListenerTypeDescription
  {
    /// <summary>
    ///   type name of the listener
    /// </summary>
    public string TypeName
    {
      get;
      set;
    }

    /// <summary>
    ///   type of the listener
    /// </summary>
    public Type Type
    {
      get => Type.GetType(TypeName, false);
      set => TypeName = value.FullName;
    }

    /// <summary>
    ///   short uqniue name of the listener
    /// </summary>
    public string Name
    {
      get;
      set;
    }
  }
}