using System.Threading.Tasks;
using SagaService.Sagas;
using Microsoft.AspNetCore.Mvc;

namespace SagaService
{
  [Route("api/[controller]")]
  public class TalkController : ControllerBase
  {
    private readonly TalkSaga _talkSaga;

    public TalkController(TalkSaga talkSaga)
    {
      _talkSaga = talkSaga;
    }

    [HttpPost("")]
    public async Task<IActionResult> StartTalk([FromBody]bool isTalkEndless)
    {
      var context = new TalkContext(isTalkEndless);
      await _talkSaga.Execute(context);

      return Ok(context.Talk);
    }
  }
}
