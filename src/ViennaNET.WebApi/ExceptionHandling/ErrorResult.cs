﻿namespace ViennaNET.WebApi.ExceptionHandling
{
  public class ErrorResult
  {
    public string Message { get; set; }
    public string ExceptionType { get; set; }
    public string StackTrace { get; set; }
  }
}