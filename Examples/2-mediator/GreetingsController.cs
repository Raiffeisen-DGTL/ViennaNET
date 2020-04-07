using MediatorService.Messages;
using Microsoft.AspNetCore.Mvc;
using ViennaNET.Mediator;

namespace MediatorService
{
  [Route("api/[controller]")]
  public class GreetingsController : ControllerBase
  {
    private readonly IMediator _mediator;

    public GreetingsController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpGet("")]
    public IActionResult Get()
    {
      var message = new GetGreetingsRequest();
      var greetings = _mediator.SendMessage<GetGreetingsRequest, GetGreetingsResult>(message);

      return Ok(greetings);
    }
  }
}
