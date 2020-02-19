using System;

namespace ViennaNET.Utils
{
  public struct ResultOf<T> : IEquatable<ResultOf<T>> where T : class
  {
    public ResultState State { get; private set; }
    public string InvalidMessage { get; private set; }
    public T Result { get; private set; }

    public static ResultOf<T> CreateSuccess(T result)
    {
      return new ResultOf<T>
      {
        State = ResultState.Success,
        InvalidMessage = null,
        Result = result
      };
    }

    public static ResultOf<T> CreateEmpty()
    {
      return new ResultOf<T>
      {
        State = ResultState.Empty,
        InvalidMessage = null,
        Result = null
      };
    }

    public static ResultOf<T> CreateInvalid(string message)
    {
      return new ResultOf<T>
      {
        State = ResultState.Invalid,
        InvalidMessage = message,
        Result = null
      };
    }

    public ResultOf<TTo> CloneFailedAs<TTo>() where TTo : class
    {
      return new ResultOf<TTo>
      {
        State = State,
        InvalidMessage = InvalidMessage,
        Result = null
      };
    }

    public bool Equals(ResultOf<T> other)
    {
      return State == other.State &&
             InvalidMessage == other.InvalidMessage &&
             Result == other.Result;
    }
  }
}
