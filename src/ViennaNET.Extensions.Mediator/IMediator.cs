// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace ViennaNET.Extensions.Mediator;

/// <summary>
///     Представляет тип "Посредник", определяющий API для обмена информацией между объектами.
/// </summary>
/// <remarks>
///     Шаблон "Посредник", в примерах реализации, определяет способ взаимодействия через "Уведомление",
///     см. описание шаблона с примерами
///     <a href="http://cpp-reference.ru/patterns/behavioral-patterns/mediator/">на русском языке</a>
///     или <a href="https://sourcemaking.com/design_patterns/mediator">первоисточник на английском</a>.
///     <br />
///     <para>
///         <see cref="IMediator" /> определяет 3 способа взаимодействия:
///         <list type="bullet">
///             <item>отправка сообщения одному получателю-обработчику</item>
///             <item>отправка сообщения-запроса одному получателю-обработчику (ожадается ответ)</item>
///             <item>отправка сообщения-события нескольким получателям-обработчикам.</item>
///         </list>
///         Такое определение способов взаимодействия объектов, не только решает исходную проблему:
///         <i>
///             обеспечить взаимодействие множества объектов, формируя при этом слабую связанность,
///             путём избавления объектов от необходимости явно ссылаться друг на друга
///         </i>
///         ,
///         но и упрощает реализацию шаблона <a href="https://ru.wikipedia.org/wiki/CQRS">CQRS</a>.
///     </para>
/// </remarks>
public interface IMediator
{
    /// <summary>
    ///     Асинхронно отправляет сообщение целевому обработчику.
    /// </summary>
    /// <param name="message">Экземпляр типа <typeparamref name="TMessage" />.</param>
    /// <param name="cancellationToken">
    ///     Экземпляр типа <see cref="CancellationToken" />, который можно использовать для отмены
    ///     работы, если она еще не началась.
    /// </param>
    /// <typeparam name="TMessage">Тип сообщения.</typeparam>
    /// <returns>
    ///     Экземпляр типа <see cref="Task" />, представляющий работу,
    ///     поставленную в очередь для выполнения в пуле потоков.
    /// </returns>
    Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Асинхронно отправляет сообщение целевому обработчику.
    /// </summary>
    /// <param name="request">Экземпляр типа <typeparamref name="TRequest" />.</param>
    /// <typeparam name="TRequest">Тип сообщения-запроса.</typeparam>
    /// <typeparam name="TResponse">
    ///     Тип ответа ожидаемого от обработчика сообщения <typeparamref name="TRequest" />.
    /// </typeparam>
    /// <param name="cancellationToken">
    ///     Экземпляр типа <see cref="CancellationToken" />, который можно использовать для отмены
    ///     работы, если она еще не началась.
    /// </param>
    /// <returns>
    ///     Экземпляр типа <see cref="Task{TResult}" />, представляющий работу,
    ///     поставленную в очередь для выполнения в пуле потоков.
    /// </returns>
    Task<TResponse?> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Асинхронно уведомляет целевых оброботчиков отправляя им <paramref name="event" />.
    /// </summary>
    /// <param name="event">Экземпляр типа <typeparamref name="TEvent" />.</param>
    /// <param name="cancellationToken">
    ///     Экземпляр типа <see cref="CancellationToken" />, который можно использовать для отмены
    ///     работы, если она еще не началась.
    /// </param>
    /// <typeparam name="TEvent">Тип сообщения-события.</typeparam>
    /// <returns>
    ///     Экземпляр типа <see cref="Task" />, представляющий работу,
    ///     поставленную в очередь для выполнения в пуле потоков.
    /// </returns>
    Task NotifyAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
}