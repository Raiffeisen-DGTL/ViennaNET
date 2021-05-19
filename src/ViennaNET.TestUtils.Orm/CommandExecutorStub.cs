using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Orm.Application;

namespace ViennaNET.TestUtils.Orm
{
  /// <summary>
  ///   CommandExecutorStub class factory method
  ///   Usage: var ces = CommandExecutorStub.Create(1);
  ///   where 1 is the result of each command execution
  ///   after call you may examine ces.CommandsCalled collection to assert executed command count and contents
  /// </summary>
  public static class CommandExecutorStub
  {
    /// <summary>
    /// Create CommandExecutorStub instance with constant command result specified
    /// </summary>
    /// <param name="result">result to return on each command execution</param>
    /// <typeparam name="T">Command type - ICommand interface realization</typeparam>
    /// <returns>CommandExecutorStub instance</returns>
    public static CommandExecutorStub<T> Create<T>(int result) where T : class, ICommand
    {
      return new CommandExecutorStub<T>(result);
    }
  }

  /// <summary>
  ///   Custom command executor stub class
  ///   Returns the result of type int passed on initialization for each call
  ///   Saves commands passed on each call (so they can be asserted after call)
  ///   Usage: var ces = CommandExecutorStub.Create(1);
  ///   where 1 is the result of each command execution
  ///   after call you may examine ces.CommandsCalled collection to assert executed command count and contents
  /// </summary>
  /// <typeparam name="T">Command type</typeparam>
  public class CommandExecutorStub<T> : ICommandExecutor<T> where T : class, ICommand
  {
    private readonly int _result;

    internal CommandExecutorStub(int result)
    {
      _result = result;
    }

    /// <summary>
    ///   Collection of commands were executed
    /// </summary>
    public ICollection<T> CommandsCalled { get; } = new List<T>();

    /// <inheritdoc />
    public int Execute(T command)
    {
      CommandsCalled.Add(command);
      return _result;
    }

    /// <inheritdoc />
    public Task<int> ExecuteAsync(T command, CancellationToken token = default)
    {
      CommandsCalled.Add(command);
      return Task.FromResult(_result);
    }
  }
}