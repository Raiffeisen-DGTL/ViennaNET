﻿using System;
using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Data;

namespace ViennaNET.WebApi
{
  public sealed partial class CompanyHostBuilder
  {
    private readonly ConcurrentDictionary<Type, int> _exceptionMapper = new();

    /// <inheritdoc />
    public ICompanyHostBuilder AddExceptionHandler<TException>(int statusCode) where TException : Exception
    {
      if (!_exceptionMapper.TryAdd(typeof(TException), statusCode))
      {
        throw new InvalidOperationException(
          $"Exception handler not registered. Exception: {typeof(TException)}; StatusCode: {statusCode}");
      }

      ConfigureApp(ConfigureExceptionHandler);

      return this;
    }

    /// <summary>
    ///   Добавить обработку ошибки
    /// </summary>
    /// <typeparam name="TException">Тип исключения</typeparam>
    /// <param name="statusCode">HTTP-код ошибки</param>
    /// <returns></returns>
    public ICompanyHostBuilder AddExceptionHandler<TException>(HttpStatusCode statusCode) where TException : Exception
    {
      return AddExceptionHandler<TException>((int)statusCode);
    }

    /// <summary>
    ///   Добавить обработку ошибки
    /// </summary>
    /// <param name="exception">Тип исключения</param>
    /// <param name="statusCode">HTTP-код ошибки</param>
    /// <returns></returns>
    public ICompanyHostBuilder AddExceptionHandler(Type exception, int statusCode)
    {
      var isExceptionType = exception.IsSubclassOf(typeof(Exception)) ||
                            exception.IsAssignableFrom(typeof(Exception));
      if (!isExceptionType)
      {
        throw new InvalidOperationException($"Type '{exception}' is not exception");
      }

      if (!_exceptionMapper.TryAdd(exception, statusCode))
      {
        throw new InvalidOperationException(
          $"Exception handler not registered. Exception: {exception}; StatusCode: {statusCode}");
      }

      ConfigureApp(ConfigureExceptionHandler);

      return this;
    }

    /// <summary>
    ///   Добавить обработку ошибки
    /// </summary>
    /// <param name="exception">Тип исключения</param>
    /// <param name="statusCode">HTTP-код ошибки</param>
    /// <returns></returns>
    public ICompanyHostBuilder AddExceptionHandler(Type exception, HttpStatusCode statusCode)
    {
      return AddExceptionHandler(exception, (int)statusCode);
    }

    private void ConfigureExceptionHandler(IApplicationBuilder builder, IConfiguration configuration,
      IHostEnvironment env, object container)
    {
      builder.UseExceptionHandler(errorApp =>
      {
        errorApp.Run(async context =>
        {
          var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();

          var exception = exceptionFeature?.Error;
          var exceptionType = exception?.GetType();

          if (exceptionType != null)
          {
            if (_exceptionMapper.TryGetValue(exceptionType, out var statusCode))
            {
              context.Response.StatusCode = statusCode;
              context.Response.ContentType = "application/json";

              var error = new ErrorDto(exception.Message);
              await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
            }
          }
        });
      });
    }
  }
}