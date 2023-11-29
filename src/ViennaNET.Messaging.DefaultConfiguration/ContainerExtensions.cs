using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Listening;
using ViennaNET.Messaging.Processing;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.DefaultConfiguration
{
  /// <summary>
  ///   Методы расширения для контейнера
  /// </summary>
  [ExcludeFromCodeCoverage]
  public static class ContainerExtensions
  {
    /// <summary>
    ///   Регистрирует дефолтный Json-сериализатор для заданного класса
    /// </summary>
    public static Container AppendSerializerJson<T>(this Container container)
    {
      container.Collection.Append<IMessageSerializer, JsonMessageSerializer<T>>(Lifestyle.Singleton);
      return container;
    }

    /// <summary>
    ///   Регистрирует дефолтный Json-десериализатор для заданного класса
    /// </summary>
    public static Container AppendDeserializerJson<T>(this Container container)
    {
      container.Collection.Append<IMessageDeserializer, JsonMessageSerializer<T>>(Lifestyle.Singleton);
      return container;
    }

    /// <summary>
    ///   Регистрирует дефолтный Xml-сериализатор для заданного класса
    /// </summary>
    public static Container AppendSerializerXml<T>(this Container container)
    {
      container.Collection.Append<IMessageSerializer, XmlMessageSerializer<T>>(Lifestyle.Singleton);
      return container;
    }

    /// <summary>
    ///   Регистрирует дефолтный Xml-десериализатор для заданного класса
    /// </summary>
    public static Container AppendDeserializerXml<T>(this Container container)
    {
      container.Collection.Append<IMessageDeserializer, XmlMessageSerializer<T>>(Lifestyle.Singleton);
      return container;
    }

    /// <summary>
    ///   Регистрирует все обработчики сообщений из указанных сборок
    /// </summary>
    public static void RegisterAllQueueProcessors(this Container container, Lifestyle lifestyle,
      params Assembly[] assemblies)
    {
      RegisterCollection(container, typeof(IProcessor), lifestyle, assemblies);
      RegisterCollection(container, typeof(IProcessorAsync), lifestyle, assemblies);
    }

    /// <summary>
    ///   Регистрирует все обработчики сообщений из указанных сборок
    /// </summary>
    public static void RegisterAllQueueProcessors(this Container container, Lifestyle lifestyle,
      IEnumerable<Assembly> assemblies)
    {
      var list = assemblies.ToList();
      RegisterCollection(container, typeof(IProcessor), lifestyle, list);
      RegisterCollection(container, typeof(IProcessorAsync), lifestyle, list);
    }

    private static void RegisterCollection(Container container, Type interfaceType, Lifestyle lifestyle,
      IEnumerable<Assembly> assemblies)
    {
      var typesToRegister = container.GetTypesToRegister(interfaceType, assemblies);

      foreach (var type in typesToRegister)
      {
        var registration = CreateRegistration(container, lifestyle, type);
        container.Collection.Append(interfaceType, registration);
      }
    }

    /// <summary>
    /// Регистрирует синхронный приемник сообщений
    /// </summary>
    /// <typeparam name="TType">Тип обработчика сообщений</typeparam>
    /// <param name="container"></param>
    /// <param name="queueName">Имя очереди</param>
    public static void RegisterMessageProcessor<TType>(this Container container,
      string queueName)
      where TType : class, IProcessor
    {
      var registration = CreateRegistration(container, Lifestyle.Singleton, typeof(TType));
      container.Collection.Append(typeof(IProcessor), registration);

      RegisterQueueListener<TType>(container, queueName, Lifestyle.Singleton);
    }

    /// <summary>
    /// Регистрирует асинхронный приемник сообщений
    /// </summary>
    /// <typeparam name="TType">Тип обработчика сообщений</typeparam>
    /// <param name="container"></param>
    /// <param name="queueName">Имя очереди</param>
    public static void RegisterAsyncMessageProcessor<TType>(this Container container,
      string queueName)
      where TType : class, IProcessorAsync
    {
      var registration = CreateRegistration(container, Lifestyle.Singleton, typeof(TType));
      container.Collection.Append(typeof(IProcessorAsync), registration);

      RegisterQueueListener<TType>(container, queueName, Lifestyle.Singleton);
    }

    private static void RegisterQueueListener<TType>(Container container,
      string queueName,
      Lifestyle currentLifestyle)
      where TType : class
    {
      container.Collection.Append<IQueueListener>(() => new QueueListener<TType>(
          (IQueueReactorFactory)container.GetRequiredService(typeof(IQueueReactorFactory)),
          (ILogger<QueueListener<TType>>)container.GetRequiredService(typeof(ILogger<QueueListener<TType>>)),
          queueName),
        currentLifestyle);
    }

    private static Registration CreateRegistration(Container container, Lifestyle lifestyle, Type type)
    {
      Registration registration;
      if (lifestyle == Lifestyle.Singleton)
      {
        registration = Lifestyle.Singleton.CreateRegistration(type, container);
      }
      else if (lifestyle == Lifestyle.Transient)
      {
        registration = Lifestyle.Transient.CreateRegistration(type, container);
      }
      else if (lifestyle == Lifestyle.Scoped)
      {
        registration = Lifestyle.Scoped.CreateRegistration(type, container);
      }
      else
      {
        throw new MessagingConfigurationException(
          $"Unsupported lifestyle {lifestyle} in RegisterAllQueueProcessors. Check your Package registrations.");
      }

      return registration;
    }
  }
}