using Microsoft.AspNetCore.Mvc;

namespace EmptyService
{
  [Route("api/[controller]")]
  public class GreetingsController : ControllerBase
  {
    [HttpGet("")]
    public IActionResult Get()
    {
      var values = new[] { "Guten Tag ViennaNET", "Hello ViennaNET!" };

      return Ok(values);
    }
  }
}
