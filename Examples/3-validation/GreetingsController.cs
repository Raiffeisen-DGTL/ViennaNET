using Microsoft.AspNetCore.Mvc;
using ValidationService.Validation;

namespace ValidationService
{
  [Route("api/[controller]")]
  public class GreetingsController : ControllerBase
  {
    private readonly IGreetingsValidationService _validationService;

    public GreetingsController(IGreetingsValidationService mediator)
    {
      _validationService = mediator;
    }

    [HttpPost("")]
    public IActionResult Post([FromBody] string greeting)
    {
      if (string.IsNullOrWhiteSpace(greeting))
      {
        return BadRequest();
      }

      var result = _validationService.ValidateGreeting(greeting);
      
      return Ok(result);
    }
  }
}
