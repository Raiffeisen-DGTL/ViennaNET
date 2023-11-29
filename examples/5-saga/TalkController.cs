using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SagaService.Sagas;

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

    /// <summary>
    ///   StartTalk
    /// </summary>
    /// <param name="interruptTalk">flag to interrupt talk</param>
    /// <returns>all talk log</returns>
    [HttpPost("")]
    public async Task<IActionResult> StartTalk([FromBody] bool interruptTalk)
    {
      var context = new TalkContext(interruptTalk);
      await _talkSaga.Execute(context);

      return Ok(context.Talk);
    }
  }
}