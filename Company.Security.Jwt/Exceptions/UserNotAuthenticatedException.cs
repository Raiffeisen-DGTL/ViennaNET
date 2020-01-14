using System;

namespace Company.Security.Jwt.Exceptions
{
  public class UserNotAuthenticatedException : Exception
  {
    public UserNotAuthenticatedException() : base("Токен авторизации отсутствует либо некорректен")
    {
    }
  }
}
