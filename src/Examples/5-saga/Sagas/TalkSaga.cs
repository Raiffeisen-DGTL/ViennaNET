using System.Threading.Tasks;
using SagaService.Services;
using ViennaNET.Sagas;
using ViennaNET.Sagas.SagaDefinition;

namespace SagaService.Sagas
{
  public class TalkSaga : SagaBase<TalkContext>
  {
    private readonly IEnglishService _englishService;
    private readonly IDeutschService _deutschService;

    public TalkSaga(IEnglishService englishService, IDeutschService deutschService)
    {
      _englishService = englishService;
      _deutschService = deutschService;

      InitSteps();
    }

    private void InitSteps()
    {
      Step("TalkInEnglish")
        .WithAction(GreetInEnglish)
        .WithCompensation(FarewellInEnglish);

      AsyncStep("TalkInDeutsch")
        .WithAction(GreetInDeutsch)
        .WithCompensation(FarewellInDeutsch);

      Step(nameof(CanTalkEnds))
        .WithAction(CanTalkEnds);
    }

    private void GreetInEnglish(TalkContext context)
    {
      var result = _englishService.Greet();
      context.Talk.Add(result);
    }

    private void FarewellInEnglish(TalkContext context)
    {
      var result = _englishService.Farewell();
      context.Talk.Add(result);
    }

    private async Task GreetInDeutsch(TalkContext context)
    {
      var result = await _deutschService.Greet();
      context.Talk.Add(result);
    }

    private async Task FarewellInDeutsch(TalkContext context)
    {
      var result = await _deutschService.Farewell();
      context.Talk.Add(result);
    }

    private void CanTalkEnds(TalkContext context)
    {
      if (context.InterruptTalk)
      {
        throw new AbortSagaExecutingException();
      }
    }
  }
}
