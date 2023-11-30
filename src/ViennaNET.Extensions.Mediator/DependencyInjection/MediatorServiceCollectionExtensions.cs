using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ViennaNET.Extensions.Mediator.DependencyInjection;

/// <summary>
///     Предоставляет методы расширения DI для регистрации посредника и обработчиков.
/// </summary>
public static class MediatorServiceCollectionExtensions
{
    /// <summary>
    ///     Добавляет службу <see cref="IMediator" /> с реализацией по умолчанию <see cref="Mediator" />.
    /// </summary>
    /// <param name="services">Ссылка на экземпляр типа <see cref="IServiceCollection" />.</param>
    /// <returns>Ссылка на экземпляр типа <see cref="IServiceCollection" />.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Возникает если значение <paramref name="services" /> = <see langword="null" />.
    /// </exception>
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        return services.AddMediator<Mediator>();
    }

    /// <summary>
    ///     Добавляет службу <see cref="IMediator" /> в DI.
    /// </summary>
    /// <param name="services">Ссылка на экземпляр типа <see cref="IServiceCollection" />.</param>
    /// <returns>Ссылка на экземпляр типа <see cref="IServiceCollection" />.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Возникает если значение <paramref name="services" /> = <see langword="null" />.
    /// </exception>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static IServiceCollection AddMediator<T>(this IServiceCollection services)
        where T : class, IMediator
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddScoped<IMediator, T>();

        return services;
    }

    /// <summary>
    ///     Добавляет в <see cref="IServiceCollection" /> указанный <typeparamref name="THandler" />,
    ///     если <typeparamref name="THandler" /> уже зарегистрирован, тогда сразу возвращает
    ///     <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">Ссылка на экземпляр типа <see cref="IServiceCollection" />.</param>
    /// <typeparam name="THandler">Тип <see cref="IMessageHandler">обработчика</see> сообщения.</typeparam>
    /// <returns>Ссылка на экземпляр типа <see cref="IServiceCollection" />.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Возникает если значение <paramref name="services" /> = <see langword="null" />.
    /// </exception>
    public static IServiceCollection TryAddHandler<THandler>(this IServiceCollection services)
        where THandler : class, IMessageHandler
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Невозможно использовать TryAddScoped, так как все обработчики регистрируются с ServiceType = IMessageHandler.
        // Используем Any, чтобы убедиться, что конкретный обработчик сообщения зарегистрирован лишь один раз.
        return services.Any(descriptor => descriptor.ImplementationType == typeof(THandler))
            ? services
            : services.AddScoped<IMessageHandler, THandler>();
    }

    /// <summary>
    ///     Добавляет в <see cref="IServiceCollection" /> все реализации <see cref="IMessageHandler" />,
    ///     обнаруженные в сборке в которой объявлен вызывающий код метод.
    /// </summary>
    /// <remarks>
    ///     Для получения обработчиков из сборки в которой объявлен вызывающий код метод испольуется
    ///     <see cref="Assembly" />.<see cref="Assembly.GetCallingAssembly" />.
    /// </remarks>
    /// <param name="services">Ссылка на экземпляр типа <see cref="IServiceCollection" />.</param>
    /// <returns>Ссылка на экземпляр типа <see cref="IServiceCollection" />.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Возникает если значение <paramref name="services" /> = <see langword="null" />.
    /// </exception>
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        return services.AddHandlersFromAssemblies(new[] { Assembly.GetCallingAssembly() });
    }

    /// <summary>
    ///     Добавляет в <see cref="IServiceCollection" /> все реализации <see cref="IMessageHandler" />,
    ///     обнаруженные в указанных сборках.
    /// </summary>
    /// <param name="services">Ссылка на экземпляр типа <see cref="IServiceCollection" />.</param>
    /// <param name="assemblies">
    ///     Ссылка на экземпляр типа <see cref="ICollection{T}" />,
    ///     содержащий коллекцию <see cref="Assembly">сборок</see> в
    ///     которых будет осуществлён поиск <see cref="IMessageHandler">обработчиков сообщений</see>.
    /// </param>
    /// <returns>Ссылка на экземпляр типа <see cref="IServiceCollection" />.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Возникает если значение <paramref name="services" /> или
    ///     <paramref name="assemblies" /> = <see langword="null" />.
    /// </exception>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static IServiceCollection AddHandlersFromAssemblies(this IServiceCollection services,
        ICollection<Assembly> assemblies)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (assemblies is null)
        {
            throw new ArgumentNullException(nameof(assemblies));
        }

        // Используется для фильтрации типов которые невозможно привести к IMessageHandler.
        static bool IsHandler(TypeInfo info)
        {
            return typeof(IMessageHandler).GetTypeInfo().IsAssignableFrom(info.AsType());
        }

        // Используется для выборка нужных типов из указанной сборки.
        static IEnumerable<Type> SelectFrom(Assembly assembly)
        {
            return assembly.DefinedTypes.Where(IsHandler).Select(info => info.AsType());
        }

        var handlerTypes = assemblies
            .SelectMany(SelectFrom)
            .Where(type => type.GetTypeInfo() is { IsAbstract: false, IsGenericTypeDefinition: false })
            .ToArray();

        foreach (var handlerType in handlerTypes)
        {
            services.AddScoped(typeof(IMessageHandler), handlerType);
        }

        return services;
    }
}