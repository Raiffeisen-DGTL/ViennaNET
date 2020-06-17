using System.Collections.Generic;

namespace SagaService.Sagas
{
  public class TalkContext
  {
    public readonly bool isTalkEndless;

    public List<string> Talk;

    public TalkContext(bool isEndlessTalk)
    {
      isTalkEndless = isEndlessTalk;

      Talk = new List<string>();
    }
  }
}
