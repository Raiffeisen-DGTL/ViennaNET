using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrmService.Entities;
using ViennaNET.Orm.Application;
using ViennaNET.Utils;

namespace OrmService
{
  [Route("api/[controller]")]
  [AllowAnonymous]
  public class GreetingsController : ControllerBase
  {
    private readonly IEntityFactoryService _efs;

    public GreetingsController(IEntityFactoryService efs)
    {
      _efs = efs.ThrowIfNull(nameof(efs));
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
      var greeting = _efs.Create<Greeting>().Get(id);

      return greeting is null
        ? (IActionResult)NotFound()
        : Ok(greeting);
    }

    [HttpPost("")]
    public IActionResult Get([FromBody] string value)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      using var uow = _efs.Create();
      var repository = _efs.Create<Greeting>();

      var newGreeting = Greeting.Create(value);

      repository.Add(newGreeting);
      uow.Commit();

      return CreatedAtAction(nameof(Get), new { id = newGreeting.Id }, newGreeting);
    }
  }
}
