namespace ViennaNET.Extensions.Mediator;

/// <summary>
///     <a
///         href="https://ru.wikipedia.org/wiki/Интерфейс-маркер_(шаблон_проектирования)">
///         Интерфейс-маркер
///     </a>
///     для обработчика сообщения.
/// </summary>
public interface IMessageHandler
{
}

/// <summary>
///     Обработчик <typeparamref name="TMessage">сообщения-команды</typeparamref> или
///     <typeparamref name="TMessage">сообщения-события</typeparamref>
///     отправленного с помощью реализации <see cref="IMediator" />.
/// </summary>
/// <remarks>
///     Обработчик - это <b>не классическая</b> реализация шаблона
///     <a href="https://sourcemaking.com/design_patterns/chain_of_responsibility/">Chain of Responsibility.</a>
///     <para>
///         Вместо базовго класса, определяющего возможность делегировать выполнение другому обработчику,
///         класс реализующий этот интерфейс должен использовать реализацию <see cref="IMediator" />
///         и сам может быть вызван через отправку <typeparamref name="TMessage" />
///         с помощью реализации <see cref="IMediator" />.
///     </para>
/// </remarks>
/// <typeparam name="TMessage">Тип сообщения.</typeparam>
public interface IMessageHandler<in TMessage> : IMessageHandler
{
    /// <summary>
    ///     Асинхронно обрабатывает сообщение, не возвращая результат.
    /// </summary>
    /// <param name="message">Ссылка на сообщение.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
    /// <returns>
    ///     Экземпляр типа <see cref="Task" />, представляющий работу,
    ///     поставленную в очередь для выполнения в пуле потоков.
    /// </returns>
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}

/// <summary>
///     Типизированный интерфейс-маркер обработчика запросов, предполагающего ответ
/// </summary>
/// <typeparam name="TMessage">Тип сообщения.</typeparam>
/// <typeparam name="TResult">Тип результата.</typeparam>
public interface IMessageHandler<in TMessage, TResult> : IMessageHandler
{
    /// <summary>
    ///     Асинхронно обрабатывает <typeparamref name="TMessage">сообщение</typeparamref>
    ///     и возвращает  результат типа <typeparamref name="TResult" /> или <see langword="null" />.
    /// </summary>
    /// <param name="message">Ссылка на сообщение.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
    /// <returns>
    ///     Экземпляр типа <see cref="Task{TResult}" />, представляющий работу,
    ///     поставленную в очередь для выполнения в пуле потоков.
    /// </returns>
    Task<TResult?> HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}