using Company.WebApi.Core.DefaultHttpSysRunner;

namespace SimpleWindowsService
{
  class Program
  {
    static void Main(string[] args)
    {
      DefaultHttpSysRunner
        .Configure()
        .BuildAndRun(args);
    }
  }
}
