using Company.Logging.Contracts;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Company.Logging.Configuration
{
  public class ListenerValidatorRepository
  {
    internal Dictionary<string, IListenerValidator> _validators =
      new Dictionary<string, IListenerValidator>(StringComparer.InvariantCultureIgnoreCase);

    private static ListenerValidatorRepository _instance;

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

    public ListenerValidatorRepository()
    {
      _validators.Add(TextFileConstants.Type, new TextFileValidator());
      _validators.Add("console", new EmptyValidator());
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