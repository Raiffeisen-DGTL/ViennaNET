using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using ViennaNET.Messaging.Exceptions;
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