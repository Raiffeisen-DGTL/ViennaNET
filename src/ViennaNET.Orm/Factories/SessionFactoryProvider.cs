using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ViennaNET.Orm.Configuration;
using ViennaNET.Orm.Exceptions;
using ViennaNET.Orm.Seedwork;
using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Connection;
using NHibernate.Context;
using NhConfiguration = NHibernate.Cfg.Configuration;
using Environment = NHibernate.Cfg.Environment;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.Log;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using Microsoft.Extensions.Logging;

namespace ViennaNET.Orm.Factories
{
  /// <inheritdoc />
  public abstract class SessionFactoryProvider : ISessionFactoryProvider
  {
    private readonly IList<Type> _fluentMappingTypes = new List<Type>();
    protected readonly ILogger _logger;
    protected readonly ConnectionInfo connectionInfo;

    protected readonly NhConfiguration nhConfig;

    protected SessionFactoryProvider(IConfiguration configuration, string nick, IInterceptor interceptor,
      ILoggerFactory loggerFactory, ILogger<SessionFactoryProvider> logger)
    {
      _logger = logger;
      connectionInfo = configuration.GetSection("db")
        .Get<ConnectionInfo[]>()
        .SingleOrDefault(ci => ci.Nick == nick && ci.DbServerType == ServerType);
      if (connectionInfo == null)
      {
        throw new InvalidOperationException($"Connection with name {nick} is not found");
      }

      nhConfig = new NhConfiguration();
      nhConfig.CurrentSessionContext<ThreadLocalSessionContext>();
      nhConfig.SetProperty(Environment.SessionFactoryName, nick);
      nhConfig.Interceptor = interceptor ?? new EmptyInterceptor();
      nhConfig.SetProperty(Environment.UseSecondLevelCache, "false");
      nhConfig.SetProperty(Environment.ConnectionProvider, typeof(DriverConnectionProvider).AssemblyQualifiedName);
      nhConfig.SetProperty(Environment.FormatSql, "true");

      NHibernateLogger.SetLoggersFactory(new NHibernateToMicrosoftLoggerFactory(loggerFactory));

      _logger.LogDebug("NHibernate configuration: "
                       + "DbNick: {SessionFactoryName}, "
                       + "UseSecondLevelCache: {UseSecondLevelCache}, "
                       + "CacheDefaultExpiration: {CacheDefaultExpiration}",
        nhConfig.GetProperty(Environment.SessionFactoryName),
        nhConfig.GetProperty(Environment.UseSecondLevelCache),
        nhConfig.GetProperty(Environment.CacheDefaultExpiration));

      Nick = nick;
      IsSkipHealthCheckEntity = connectionInfo.IsSkipHealthCheckEntity;
    }

    /// <summary>
    ///   Тип БД
    /// </summary>
    public abstract string ServerType { get; }

    /// <inheritdoc />
    /// <remarks>
    ///   При создании фабрики сессий используется <see cref="FluentNHibernate" />
    /// </remarks>
    public virtual ISessionFactory GetSessionFactory()
    {
      var fluentConfig = Fluently.Configure(nhConfig);
      foreach (var type in _fluentMappingTypes)
      {
        fluentConfig.Mappings(x => x.FluentMappings.Add(type));
      }

      _fluentMappingTypes.Clear();
      return fluentConfig.BuildSessionFactory();
    }

    /// <inheritdoc />
    public string Nick { get; }

    /// <inheritdoc />
    public bool IsSkipHealthCheckEntity { get; }

    /// <inheritdoc />
    /// <remarks>
    ///   Для сущностей сначала осуществляется поиск fluent-маппинга, в случае
    ///   неудачи ищется hbm.xml-маппинг.
    /// </remarks>
    public ISessionFactoryProvider AddClass(Type type, Assembly assembly = null)
    {
      var isIEntityImplemented = type.GetInterfaces()
        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityKey<>));
      if (!isIEntityImplemented)
      {
        throw new
          EntityRegistrationException(
            $"All entities should implement IEntityKey interface. The entity of type {type} does not implement it.");
      }

      return FindMapping(type, assembly);
    }

    public ISessionFactoryProvider AddEvent(Type type, Assembly assembly = null)
    {
      var isIEntityImplemented = type.GetInterfaces()
        .Any(i => i == typeof(IIntegrationEvent));
      if (!isIEntityImplemented)
      {
        throw new
          EntityRegistrationException(
            $"All entities should implement IEntityKey interface. The entity of type {type} does not implement it.");
      }

      return FindMapping(type, assembly);
    }

    /// <remarks>
    ///   Cначала осуществляется поиск fluent-маппинга, в случае
    ///   неудачи ищется hbm.xml-маппинг.
    /// </remarks>
    private ISessionFactoryProvider FindMapping(Type type, Assembly assembly = null)
    {
      assembly ??= type.Assembly;

      var fluentMappings = assembly.GetTypes()
        .Where(x => x.IsSubclassOf(typeof(ClasslikeMapBase<>).MakeGenericType(type)))
        .ToList();

      if (fluentMappings.Count > 1)
      {
        throw new EntityMappingRegistrationException($"There are more than one fluent mapping for {type.FullName}.");
      }

      var fluentMapping = fluentMappings.SingleOrDefault();

      if (fluentMapping != null)
      {
        _fluentMappingTypes.Add(fluentMapping);
        _logger.LogDebug("An entity with the {TypeFullName} has been registered with the fluent mapping",
          type.FullName);
      }
      else
      {
        var typeManifestName = assembly.GetManifestResourceNames()
          .SingleOrDefault(x => x.Contains($"{type.FullName}.hbm.xml"));

        if (string.IsNullOrEmpty(typeManifestName))
        {
          throw new EntityMappingRegistrationException($"There is no .hbm.xml mapping for {type.FullName}.");
        }

        nhConfig.AddResource(typeManifestName, assembly);
        _logger.LogDebug("An entity with the {TypeFullName} and the mapping {TypeManifestName} has been registered",
          type.FullName,
          typeManifestName);
      }

      return this;
    }
  }
}