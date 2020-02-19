using System;

namespace ViennaNET.Security.Jwt.Exceptions
{
  public class UserNotAuthenticatedException : Exception
  {
    public UserNotAuthenticatedException() : base("Токен авторизации отсутствует либо некорректен")
    {
    }
  }
}
