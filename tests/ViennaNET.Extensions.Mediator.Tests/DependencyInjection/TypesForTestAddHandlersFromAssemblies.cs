using System.Diagnostics.CodeAnalysis;
using ViennaNET.Extensions.Mediator.DependencyInjection;
using ViennaNET.Extensions.Mediator.Tests.Models;

namespace ViennaNET.Extensions.Mediator.Tests.DependencyInjection;

/// <summary>
///     Требуется для тестирования <see cref="MediatorServiceCollectionExtensions.AddHandlers" />,
///     который добавляет обработчики из указанных сборок.
///     Для проверки предиката который передаётся в Where.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Используется для тестирования, и не реализуется поведения.")]
public abstract class AbstractTypeForTestAddHandlersFromAssemblies : IMessageHandler<Message>
{
    public Task HandleAsync(Message message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     Требуется для тестирования <see cref="MediatorServiceCollectionExtensions.AddHandlers" />,
///     который добавляет обработчики из указанных сборок.
///     Для проверки предиката который передаётся в Where.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Используется для тестирования, и не реализуется поведения.")]
public class AbstractTypeForTestAddHandlersFromAssemblies<T> : IMessageHandler<T>
{
    public Task HandleAsync(T message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}