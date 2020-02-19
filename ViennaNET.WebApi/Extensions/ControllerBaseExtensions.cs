using ViennaNET.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ViennaNET.WebApi.Extensions
{
  public static class ControllerBaseExtensions
  {
    /// <summary>
    /// Позволяет обработать <see cref="ResultOf{T}"/> как HTTP ответ
    /// </summary>
    /// <param name="controller">Контроллер</param>
    /// <param name="result">Результат для обработки</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Возвращается NotFound (404), BadRequest (400) с текстом ошибки или OK (200) с объектом</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ActionResult HandleAsOk<T>(this ControllerBase controller, ResultOf<T> result) where T : class
    {
      switch (result.State)
      {
        case ResultState.Empty: return new NotFoundResult();
        case ResultState.Invalid: return new BadRequestObjectResult(result.InvalidMessage);
        case ResultState.Success: return new OkObjectResult(result.Result);
        default: throw new ArgumentOutOfRangeException(nameof(result));
      }
    }
  }
}
