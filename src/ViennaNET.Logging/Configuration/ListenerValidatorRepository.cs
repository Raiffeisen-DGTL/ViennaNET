using System;
using System.Collections.Generic;
using System.Threading;
using ViennaNET.Logging.Contracts;

namespace ViennaNET.Logging.Configuration
{
  public class ListenerValidatorRepository
  {
    private static ListenerValidatorRepository _instance;

    internal Dictionary<string, IListenerValidator> _validators =
      new(StringComparer.InvariantCultureIgnoreCase);

    public ListenerValidatorRepository()
    {
      _validators.Add(TextFileConstants.Type, new TextFileValidator());
      _validators.Add("console", new EmptyValidator());
    }

    public static ListenerValidatorRepository Instance
    {
      get
      {
        if (_instance == null)
        {
          Interlocked.CompareExchange(ref _instance, new ListenerValidatorRepository(), null);
        }

        return _instance;
      }
    }

    public IListenerValidator GetValidator(string name)
    {
      if (_validators.ContainsKey(name))
      {
        return _validators[name];
      }

      return null;
    }
  }
}