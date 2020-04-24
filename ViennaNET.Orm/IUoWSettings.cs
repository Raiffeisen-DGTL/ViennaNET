using System.Data;

namespace ViennaNET.Orm.Application
{
  internal interface IUoWSettings
  {
    IsolationLevel IsolationLevel { get; }
    bool AutoControl { get; }
    bool CloseSessions { get; }
  }
}
