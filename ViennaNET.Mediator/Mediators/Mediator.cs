using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Logging;
using ViennaNET.Mediator.Exceptions;
using ViennaNET.Mediator.Pipeline;
using ViennaNET.Mediator.Pipelines;
using ViennaNET.Mediator.Seedwork;
using ViennaNET.Utils;

namespace ViennaNET.Mediator.Mediators
{
  /// <summary>
  ///   Реализация медиатора с возможностью регистрации обработчиков
  /// </summary>
  public class Mediator : IMediator, IMessageRecipientsRegistrar, IPipelineProcessorsRegistrar
  {
    private readonly IPreProcessorService _preProcessorService;
    private ConcurrentBag<IMessageHandlerAsync> _asyncHandlers;
    private ConcurrentBag<IMessageHandler> _handlers;

    /// <summary>
    ///   Инициализирует медиатор с пустыми коллекциями обработчиков
    ///   <see cref="IEnumerable{T}" />.
    /// </summary>
    public Mediator(IPreProcessorService preProcessorService)
    {
      _preProcessorService = preProcessorService.ThrowIfNull(nameof(preProcessorService));

      _handlers = new ConcurrentBag<IMessageHandler>();
      _asyncHandlers = new ConcurrentBag<IMessageHandlerAsync>();
    }

    /// <inheritdoc />
    /// <exception cref="T:System.ArgumentNullException" />
    /// <exception cref="T:ViennaNET.Mediator.Exceptions.UnsupportedTypeMessageException" />
    public async Task SendMessageAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
      where TMessage : class, IMessage
    {
      await _preProcessorService.ExecuteAllPreProcessorsAsync(message, cancellationToken);
      try
      {
        switch (message)
        {
          case null:
            throw new ArgumentNullException(nameof(message));
          case IEvent _:
            await PublishEventAsync(message, cancellationToken);
            break;
          case ICommand _:
            await ExecuteCommandAsync(message, cancellationToken);
            break;
          default:
            throw new UnsupportedTypeMessageException($"Unsupported message type: {message.GetType()}.");
        }
      }
      catch (Exception e)
      {
        LogSendError(e);
        throw;
      }
    }

    /// <inheritdoc />
    /// <exception cref="T:System.ArgumentNullException" />
    /// <exception cref="T:ViennaNET.Mediator.Exceptions.UnsupportedTypeMessageException" />
    public async Task<TResponse> SendMessageAsync<TMessage, TResponse>(TMessage message, CancellationToken cancellationToken = default)
      where TMessage : class, IMessage
    {
      await _preProcessorService.ExecuteAllPreProcessorsAsync(message, cancellationToken);
      try
      {
        switch (message)
        {
          case null:
            throw new ArgumentNullException(nameof(message));
          case IRequest _:
            return await SendRequestAsync<TMessage, TResponse>(message, cancellationToken);
          default:
            throw new UnsupportedTypeMessageException($"Unsupported message type: {message.GetType()}.");
        }
      }
      catch (Exception e)
      {
        LogSendError(e);
        throw;
      }
    }

    /// <inheritdoc />
    /// <exception cref="T:System.ArgumentNullException" />
    /// <exception cref="T:ViennaNET.Mediator.Exceptions.UnsupportedTypeMessageException" />
    public void SendMessage<TMessage>(TMessage message) where TMessage : class, IMessage
    {
      _preProcessorService.ExecuteAllPreProcessors(message);
      try
      {
        switch (message)
        {
          case null:
            throw new ArgumentNullException(nameof(message));
          case IEvent _:
            PublishEvent(message);
            break;
          case ICommand _:
            ExecuteCommand(message);
            break;
          default:
            throw new UnsupportedTypeMessageException($"Unsupported message type: {message.GetType()}.");
        }
      }
      catch (Exception e)
      {
        LogSendError(e);
        throw;
      }
    }

    /// <inheritdoc />
    /// <exception cref="T:System.ArgumentNullException" />
    /// <exception cref="T:ViennaNET.Mediator.Exceptions.UnsupportedTypeMessageException" />
    public TResponse SendMessage<TMessage, TResponse>(TMessage message) where TMessage : class, IMessage
    {
      _preProcessorService.ExecuteAllPreProcessors(message);
      try
      {
        switch (message)
        {
          case null:
            throw new ArgumentNullException(nameof(message));
          case IRequest _:
            return SendRequest<TMessage, TResponse>(message);
          default:
            throw new UnsupportedTypeMessageException($"Unsupported message type: {message.GetType()}.");
        }
      }
      catch (Exception e)
      {
        LogSendError(e);
        throw;
      }
    }

    /// <inheritdoc />
    public void RegisterEventListener<TEvent, TEventListener>(TEventListener listener) where TEvent : class, IEvent
      where TEventListener : IMessageHandler<TEvent>
    {
      if (listener == null)
      {
        throw new ArgumentNullException(nameof(listener));
      }

      if (_handlers.SingleOrDefault(handler => handler?.GetType() == listener.GetType()) != null)
      {
        throw new ArgumentException($"Listener with type of {listener.GetType()} already registered.", nameof(listener));
      }

      _handlers.Add(listener);
    }

    /// <inheritdoc />
    public void RegisterCommandReceiver<TCommand, TCommandReceiver>(TCommandReceiver receiver) where TCommand : class, ICommand
      where TCommandReceiver : IMessageHandler<TCommand>
    {
      if (receiver == null)
      {
        throw new ArgumentNullException(nameof(receiver));
      }

      if (_handlers.SingleOrDefault(handler => handler?.GetType() == receiver.GetType()) != null)
      {
        throw new ArgumentException($"Receiver with type of {receiver.GetType()} already registered.", nameof(receiver));
      }

      _handlers.Add(receiver);
    }

    /// <inheritdoc />
    public void RegisterRequestHandler<TRequest, TResponse, TRequestHandler>(TRequestHandler registeredHandler)
      where TRequest : class, IRequest where TRequestHandler : IMessageHandler<TRequest, TResponse>
    {
      if (registeredHandler == null)
      {
        throw new ArgumentNullException(nameof(registeredHandler));
      }

      if (_handlers.SingleOrDefault(handler => handler?.GetType() == registeredHandler.GetType()) != null)
      {
        throw new ArgumentException($"The request {typeof(TRequest).FullName} handler is already registered.", nameof(registeredHandler));
      }

      _handlers.Add(registeredHandler);
    }

    /// <inheritdoc />
    public void Register(IEnumerable<IMessageHandler> messageRecipients, IEnumerable<IMessageHandlerAsync> asyncMessageHandlers)
    {
      _handlers = new ConcurrentBag<IMessageHandler>(messageRecipients);
      _asyncHandlers = new ConcurrentBag<IMessageHandlerAsync>(asyncMessageHandlers);
    }

    public void RegisterMessagePreProcessor<TMessage, TPipelineProcessor>(TPipelineProcessor registerPreProcessor, int order)
      where TMessage : class, IMessage where TPipelineProcessor : IMessagePreProcessor<TMessage>
    {
      _preProcessorService.RegisterMessagePreProcessor<TMessage, TPipelineProcessor>(registerPreProcessor, order);
    }

    public void RegisterBroadcastPreProcessor<TPipelineProcessor>(TPipelineProcessor registerPreProcessor, int order)
      where TPipelineProcessor : IBroadcastPreProcessor
    {
      _preProcessorService.RegisterBroadcastPreProcessor(registerPreProcessor, order);
    }

    private static void LogSendError(Exception e)
    {
      Logger.LogErrorFormat(e, "Could not send message, an one or more error occured");
    }

    private Task ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
      where TCommand : class, IMessage
    {
      var commandHandlers = _asyncHandlers.OfType<IMessageHandlerAsync<TCommand>>()
                                          .ToList();

      if (!commandHandlers.Any())
      {
        throw new ExecuteCommandException($"For the {nameof(command)} does not exist {nameof(IMessageHandlerAsync<TCommand>)}.");
      }

      if (commandHandlers.Count > 1)
      {
        throw new ExecuteCommandException($"For the {nameof(command)} there cannot be more than one handler.");
      }

      return commandHandlers.Single()
                            .HandleAsync(command, cancellationToken);
    }

    private Task PublishEventAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : class, IMessage
    {
      var eventHandlers = _asyncHandlers.OfType<IMessageHandlerAsync<TEvent>>()
                                        .ToList();

      if (!eventHandlers.Any())
      {
        throw new PublishEventException($"For {nameof(evt)} does not exist {nameof(IMessageHandlerAsync<TEvent>)}");
      }

      return Task.WhenAll(eventHandlers.Select(handler => handler.HandleAsync(evt, cancellationToken)));
    }

    private Task<TResponse> SendRequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
      where TRequest : class, IMessage
    {
      var requestHandler = _asyncHandlers.OfType<IMessageHandlerAsync<TRequest, TResponse>>()
                                         .ToList();

      if (!requestHandler.Any())
      {
        throw new SendRequestException($"For the {nameof(request)} does not exist {nameof(IMessageHandlerAsync<TRequest, TResponse>)}.");
      }

      if (requestHandler.Count > 1)
      {
        throw new SendRequestException($"There can not be more than one handler for the {nameof(request)}.");
      }

      return requestHandler.Single()
                           .HandleAsync(request, cancellationToken);
    }

    private void ExecuteCommand<TCommand>(TCommand command) where TCommand : class, IMessage
    {
      var commandHandlers = _handlers.OfType<IMessageHandler<TCommand>>()
                                     .ToList();

      if (!commandHandlers.Any())
      {
        throw new ExecuteCommandException($"For the {nameof(command)} does not exist {nameof(IMessageHandler<TCommand>)}.");
      }

      if (commandHandlers.Count > 1)
      {
        throw new ExecuteCommandException($"For the {nameof(command)} there cannot be more than one handler.");
      }

      commandHandlers.Single()
                     .Handle(command);
    }

    private void PublishEvent<TEvent>(TEvent evt) where TEvent : class, IMessage
    {
      var eventHandlers = _handlers.OfType<IMessageHandler<TEvent>>()
                                   .ToList();

      if (!eventHandlers.Any())
      {
        throw new PublishEventException($"For {nameof(evt)} does not exist {nameof(IMessageHandler<TEvent>)}");
      }

      eventHandlers.ToList()
                   .ForEach(handler => handler.Handle(evt));
    }

    private TResponse SendRequest<TRequest, TResponse>(TRequest request) where TRequest : class, IMessage
    {
      var requestHandler = _handlers.OfType<IMessageHandler<TRequest, TResponse>>()
                                    .ToList();

      if (!requestHandler.Any())
      {
        throw new SendRequestException($"For the {nameof(request)} does not exist {nameof(IMessageHandler<TRequest, TResponse>)}.");
      }

      if (requestHandler.Count > 1)
      {
        throw new SendRequestException($"There can not be more than one handler for the {nameof(request)}.");
      }

      return requestHandler.Single()
                           .Handle(request);
    }
  }
}
