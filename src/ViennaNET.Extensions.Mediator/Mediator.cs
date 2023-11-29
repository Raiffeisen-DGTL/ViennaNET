using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace ViennaNET.Extensions.Mediator;

/// <inheritdoc />
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public class Mediator : IMediator
{
  /// <summary>
  ///     Инициализирует новый экземпляр класса <see cref="Mediator" />.
  /// </summary>
  /// <param name="handlers">Коллекция обработчиков сообщений <see cref="IMessageHandler" />.</param>
  /// <exception cref="ArgumentNullException">
  ///     Возникает если значение параметра <paramref name="handlers" /> = <see langword="null" />.
  /// </exception>
  public Mediator(IEnumerable<IMessageHandler> handlers)
  {
    Handlers = new ConcurrentBag<IMessageHandler>(
      handlers ?? throw new ArgumentNullException(nameof(handlers)));
  }

  /// <value>
  ///     Получает ссылку на экземпляр типа <see cref="ConcurrentBag{T}" />,
  ///     содержащий коллекцию <see cref="IMessageHandler">обработчиков</see>.
  /// </value>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  protected ConcurrentBag<IMessageHandler> Handlers { get; }

  /// <inheritdoc />
  /// <exception cref="ArgumentNullException">
  ///     Возникает если значение параметра <paramref name="message" /> = <see langword="null" />.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///     Возникает если для <paramref name="message" /> нет обработчика или их больше 1.
  /// </exception>
  public virtual Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
  {
    if (message is null)
    {
      throw new ArgumentNullException(nameof(message));
    }

    return GetHandler<IMessageHandler<TMessage>>().HandleAsync(message, cancellationToken);
  }

  /// <inheritdoc />
  /// <exception cref="ArgumentNullException">
  ///     Возникает если значение параметра <paramref name="request" /> = <see langword="null" />.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///     Возникает если для <paramref name="request" /> нет обработчика или их больше 1.
  /// </exception>
  public virtual Task<TResponse?> SendAsync<TRequest, TResponse>(
    TRequest request, CancellationToken cancellationToken = default)
  {
    if (request is null)
    {
      throw new ArgumentNullException(nameof(request));
    }

    return GetHandler<IMessageHandler<TRequest, TResponse>>().HandleAsync(request, cancellationToken);
  }

  /// <inheritdoc />
  /// <exception cref="ArgumentNullException">
  ///     Возникает если значение параметра <paramref name="event" /> = <see langword="null" />.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///     Возникает если для <paramref name="event" /> нет обработчиков.
  /// </exception>
  public virtual Task NotifyAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
  {
    if (@event is null)
    {
      throw new ArgumentNullException(nameof(@event));
    }

    return Task.WhenAll(GetHandlers<IMessageHandler<TEvent>>()
      .Select(handler => handler.HandleAsync(@event, cancellationToken)));
  }

  private IEnumerable<THandler> GetHandlers<THandler>() where THandler : IMessageHandler
  {
    static string FormatError()
    {
      return
        $"A handlers of {typeof(THandler)} for the message type " +
        $"{typeof(THandler).GenericTypeArguments[0]} was not found.";
    }

    return Handlers.OfType<THandler>().ToArray() is { Length: > 0 } handlers
      ? handlers
      : throw new InvalidOperationException(FormatError());
  }

  private THandler GetHandler<THandler>() where THandler : IMessageHandler
  {
    static string FormatError()
    {
      return
        $"A handler of {typeof(THandler)} for the message type " +
        $"{typeof(THandler).GenericTypeArguments[0]} was not found or found more than one handlers.";
    }

    return Handlers.OfType<THandler>().SingleOrDefault()
           ?? throw new InvalidOperationException(FormatError());
  }
}