using System.Threading.Tasks;
using MessagingService.Features.Send.Services;
using Microsoft.AspNetCore.Mvc;

namespace MessagingService.Features.Send
{
  [Route("api/[controller]")]
  public class MessageController : ControllerBase
  {
    private readonly ISendingService _sendingService;

    public MessageController(ISendingService sendingService)
    {
      _sendingService = sendingService;
    }

    [HttpPost("")]
    public async Task<IActionResult> Post([FromBody]string text)
    {
      await _sendingService.Send(text);

      return Ok();
    }
  }
}
