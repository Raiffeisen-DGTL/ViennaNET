using System.Collections.Generic;

namespace SagaService.Sagas
{
  public class TalkContext
  {
    public TalkContext(bool interruptTalk)
    {
      InterruptTalk = interruptTalk;

      Talk = new List<string>();
    }

    public bool InterruptTalk { get; }

    public List<string> Talk { get; }
  }
}