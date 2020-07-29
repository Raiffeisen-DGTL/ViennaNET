using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrmService.Queries;
using ViennaNET.Orm.Application;
using ViennaNET.Utils;

namespace EmptyService
{
  [Route("api/[controller]")]
  [AllowAnonymous]
  public class GreetingsQueriesController : ControllerBase
  {
    private readonly IEntityFactoryService _efs;

    public GreetingsQueriesController(IEntityFactoryService efs)
    {
      _efs = efs.ThrowIfNull(nameof(efs));
    }

    [HttpGet("")]
    public IActionResult Get([FromQuery] int? id)
    {
      var result = _efs.CreateCustomQueryExecutor<GreetingsValuesQueryItem>().CustomQuery(new GreetingsValuesQuery(id));

      return Ok(result);
    }
  }
}
