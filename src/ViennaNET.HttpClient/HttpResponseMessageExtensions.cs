﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ViennaNET.Utils;

namespace ViennaNET.HttpClient
{
  public static class HttpResponseMessageExtensions
  {
    private static readonly IEnumerable<MediaTypeFormatter> formatters = new List<MediaTypeFormatter>
    {
      new JsonMediaTypeFormatter(), new FormUrlEncodedMediaTypeFormatter()
    };

    private static string GetBadRequestMessage(string error)
    {
      try
      {
        var obj = JObject.Parse(error);
        return (string)obj["Message"];
      }
      catch
      {
        return error;
      }
    }

    /// <summary>
    ///   При успешном коде ответа, возвращает объект типа <see cref="ResultOf{T}" /> в состоянии 'Success'.
    ///   При ответе <see cref="HttpStatusCode.NotFound" /> возвращает <see cref="ResultOf{T}" /> в состоянии 'Empty'.
    ///   При ответе <see cref="HttpStatusCode.BadRequest" /> возвращает <see cref="ResultOf{T}" /> в состоянии 'Invalid'.
    /// </summary>
    /// <exception cref="InvalidOperationException">При неизвестных ответах</exception>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <returns>Объект типа <see cref="ResultOf{T}" />.</returns>
    public static async Task<ResultOf<T>> HandleAsync<T>(this HttpResponseMessage response) where T : class
    {
      if (response.IsSuccessStatusCode)
      {
        var dto = await response.Content.ReadAsAsync<T>(formatters);
        return ResultOf<T>.CreateSuccess(dto);
      }

      switch (response.StatusCode)
      {
        case HttpStatusCode.NotFound:
          return ResultOf<T>.CreateEmpty();
        case HttpStatusCode.BadRequest:
          var error = await response.Content.ReadAsStringAsync();
          var message = GetBadRequestMessage(error);
          return ResultOf<T>.CreateInvalid(message);
        default:
          var exception = await response.Content.ReadAsStringAsync();
          throw new InvalidOperationException(exception);
      }
    }
  }
}