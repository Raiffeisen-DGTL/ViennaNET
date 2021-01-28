using System;

namespace ViennaNET.Utils
{
  /// <summary>
  /// Represents operation result
  /// Dependent to state, may be empty, success with result specified, or invalid with invalid message specified
  /// </summary>
  /// <typeparam name="T">Type of result</typeparam>
  public struct ResultOf<T> : IEquatable<ResultOf<T>> where T : class
  {
    /// <summary>
    /// Result state - either ResultState.Empty, ResultState.Success with Result specified or ResultState.Invalid with InvalidMessage specified
    /// </summary>
    public ResultState State { get; private set; }
    
    /// <summary>
    /// Invalid message specified in case of Invalid state
    /// </summary>
    public string? InvalidMessage { get; private set; }
    
    /// <summary>
    /// Result - in case of Success state
    /// </summary>
    public T? Result { get; private set; }

    /// <summary>
    /// Create in Success state with result specified
    /// </summary>
    /// <param name="result">result content</param>
    /// <returns>Created result</returns>
    public static ResultOf<T> CreateSuccess(T result)
    {
      return new ResultOf<T>
      {
        State = ResultState.Success,
        InvalidMessage = null,
        Result = result
      };
    }

    /// <summary>
    /// Create in empty state
    /// </summary>
    /// <returns>Created result</returns>
    public static ResultOf<T> CreateEmpty()
    {
      return new ResultOf<T>
      {
        State = ResultState.Empty,
        InvalidMessage = null,
        Result = null
      };
    }

    /// <summary>
    /// Create in Invalid state with invalid message specified
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ResultOf<T> CreateInvalid(string message)
    {
      return new ResultOf<T>
      {
        State = ResultState.Invalid,
        InvalidMessage = message,
        Result = null
      };
    }

    /// <summary>
    ///  Clone to other generic type if result us not specified 
    /// </summary>
    /// <typeparam name="TTo">Target generic type</typeparam>
    /// <returns>Clone result</returns>
    public ResultOf<TTo> CloneFailedAs<TTo>() where TTo : class
    {
      return new ResultOf<TTo>
      {
        State = State,
        InvalidMessage = InvalidMessage,
        Result = null
      };
    }

    /// <inheritdoc />
    public bool Equals(ResultOf<T> other)
    {
      return State == other.State &&
             InvalidMessage == other.InvalidMessage &&
             Result == other.Result;
    }
  }
}
