using ViennaNET.Mediator.Pipelines;
using ViennaNET.Mediator.Seedwork;
using SimpleInjector;

namespace ViennaNET.Mediator.DefaultConfiguration
{
  public static class SimpleInjectorExtensions
  {
    /// <summary>
    /// Регистрирует обработчик для синхронного взаимодействия
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения</typeparam>
    /// <typeparam name="TResponse">Тип результата</typeparam>
    /// <typeparam name="THandler">Тип обработчика</typeparam>
    /// <param name="container">DI-контейнер SimpleInjector-а</param>
    /// <returns></returns>
    public static Container RegisterHandler<TMessage, TResponse, THandler>(this Container container) where TMessage : class, IMessage
      where THandler : class, IMessageHandler<TMessage, TResponse>
    {
      container.Collection.Append<IMessageHandler, THandler>(Lifestyle.Singleton);
      container.Collection.Append<IMessageHandler<TMessage, TResponse>, THandler>(Lifestyle.Singleton);

      return container;
    }

    /// <summary>
    /// Регистрирует обработчик для асинхронного взаимодействия
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения</typeparam>
    /// <typeparam name="TResponse">Тип результата</typeparam>
    /// <typeparam name="THandler">Тип обработчика</typeparam>
    /// <param name="container">DI-контейнер SimpleInjector-а</param>
    /// <returns></returns>
    public static Container RegisterAsyncHandler<TMessage, TResponse, THandler>(this Container container) where TMessage : class, IMessage
      where THandler : class, IMessageHandlerAsync<TMessage, TResponse>
    {
      container.Collection.Append<IMessageHandlerAsync, THandler>(Lifestyle.Singleton);
      container.Collection.Append<IMessageHandlerAsync<TMessage, TResponse>, THandler>(Lifestyle.Singleton);

      return container;
    }

    /// <summary>
    /// Регистрирует обработчик как для синхронного, так и для асинхронного взаимодействия
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения</typeparam>
    /// <typeparam name="TResponse">Тип результата</typeparam>
    /// <typeparam name="THandler">Тип обработчика</typeparam>
    /// <param name="container">DI-контейнер SimpleInjector-а</param>
    /// <returns></returns>
    public static Container RegisterFullHandler<TMessage, TResponse, THandler>(this Container container) where TMessage : class, IMessage
      where THandler : class, IMessageHandler<TMessage, TResponse>, IMessageHandlerAsync<TMessage, TResponse>
    {
      container.RegisterHandler<TMessage, TResponse, THandler>();
      container.RegisterAsyncHandler<TMessage, TResponse, THandler>();

      return container;
    }

    /// <summary>
    /// Registers for message pre-processor.
    /// </summary>
    /// <typeparam name="TMessage">The type of message <see cref="IMessage"/>.</typeparam>
    /// <typeparam name="TMessagePreProcessor">The type of pre-processor.</typeparam>
    /// <param name="order">Order of pre-processor for ordered execution.</param>
    /// <param name="container">The container.</param>
    public static void RegisterMessagePreProcessor<TMessage, TMessagePreProcessor>(this Container container, int order = 0)
      where TMessage : class, IMessage where TMessagePreProcessor : IMessagePreProcessor<TMessage>
    {
      container.Register(typeof(TMessagePreProcessor));
      container.Register(() => new PreProcessorOrder<TMessagePreProcessor> { Order = order });
      container.Register<PipelineMessagePreProcessorRegistrar<TMessage, TMessagePreProcessor>>();
    }

    /// <summary>
    /// Registers broadcast pre-processor.
    /// </summary>
    /// <typeparam name="TMessagePreProcessor">The type of pre-processor.</typeparam>
    /// <param name="container">The container.</param>
    /// <param name="order">Order of pre-processor for ordered execution.</param>
    public static void RegisterBroadcastPreProcessor<TMessagePreProcessor>(this Container container, int order = 0)
      where TMessagePreProcessor : IBroadcastPreProcessor
    {
      container.Register(typeof(TMessagePreProcessor));
      container.Register(() => new PreProcessorOrder<TMessagePreProcessor> { Order = order });
      container.Register<PipelineBroadcastPreProcessorRegistrar<TMessagePreProcessor>>();
    }
  }
}
