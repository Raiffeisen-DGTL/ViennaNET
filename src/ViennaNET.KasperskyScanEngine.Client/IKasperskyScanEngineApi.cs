using ViennaNET.KasperskyScanEngine.Client.Models;

namespace ViennaNET.KasperskyScanEngine.Client;

/// <summary>
///     Представляет API
///     <a href="https://support.kaspersky.com/help/ScanEngine/2.1/ru-RU/181169.htm">
///         Kaspersky Scan Engine.
///     </a>
/// </summary>
public interface IKasperskyScanEngineApi
{
    /// <summary>
    ///     Имя экземпляра параметровк конфигурации сереализатора Json.
    /// </summary>
    public const string JsonSerializerOptionsName = "KSE";

    /// <summary>
    ///     Сканирует файл, к которому у Kaspersky Scan Engine есть доступ.
    /// </summary>
    /// <param name="request">Ссылка на экземпляр <see cref="ScanFileRequest" />.</param>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="ScanFileResponse" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<ScanFileResponse?> PostScanFileAsync(ScanFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Сканирует блок памяти – содержимое сканируемого файла в кодировке Base64.
    /// </summary>
    /// <remarks>
    ///     При сканировании системной памяти используйте только режим очистки KAV_SKIP.
    ///     Kaspersky Scan Engine не может лечить или удалять файлы в этом режиме.
    /// </remarks>
    /// <param name="stream">Ссылка на экземпляр потока с содержимым для сканирования.</param>
    /// <param name="name">Имя сканируемого файла.</param>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="ScanMemoryResponse" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<ScanMemoryResponse?> PostScanMemoryAsync(Stream stream, string? name,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Сканирует блок памяти – содержимое сканируемого файла в кодировке Base64.
    /// </summary>
    /// <remarks>
    ///     При сканировании системной памяти используйте только режим очистки KAV_SKIP.
    ///     Kaspersky Scan Engine не может лечить или удалять файлы в этом режиме.
    /// </remarks>
    /// <param name="request">Ссылка на экземпляр <see cref="ScanMemoryRequest" />.</param>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="ScanMemoryResponse" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<ScanMemoryResponse?> PostScanMemoryAsync(ScanMemoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Проверяет URL-адрес.
    /// </summary>
    /// <param name="request">Ссылка на экземпляр <see cref="CheckUrlRequest" />.</param>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="CheckUrlResponse" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<CheckUrlResponse?> PostCheckUrlAsync(CheckUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Получает дату выпуска антивирусных баз.
    ///     Вы можете использовать этот метод, чтобы проверить, запущен ли kavhttpd.
    /// </summary>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="BasesDateResponse" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<BasesDateResponse?> GetBaseDateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Получает текущую версию KAV SDK. Вы можете использовать этот метод, чтобы проверить, запущен ли kavhttpd.
    /// </summary>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="VersionResponse" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<VersionResponse?> GetVersionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Получает информацию о текущем файле ключа или коде активации.
    ///     Вы можете использовать этот метод, чтобы проверить, запущен ли kavhttpd.
    /// </summary>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="LicenseInfoResponse" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<LicenseInfoResponse?> GetLicenseInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Получает накопленную статистику.
    /// </summary>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="StatisticsResponse" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<StatisticsResponse?> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Очищает накопленную статистику.
    /// </summary>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="ErrorResponse" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<ErrorResponse?> PostClearStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Обновляет антивирусные базы.
    /// </summary>
    /// <remarks>
    ///     Запрос может быть выполнен, только если HTTP-клиент и Kaspersky Scan Engine установлены на одном компьютере.
    ///     Если вы пошлете этот запрос с другого компьютера, Kaspersky Scan Engine вернет 405 Method Not Allowed.
    /// </remarks>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="UpdateStatusResponse" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<UpdateStatusResponse?> PostUpdateStartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Получает статус процесса обновления базы данных.
    /// </summary>
    /// <remarks>
    ///     Запрос может быть выполнен, только если HTTP-клиент и Kaspersky Scan Engine установлены на одном компьютере.
    ///     Если вы пошлете этот запрос с другого компьютера, Kaspersky Scan Engine вернет 405 Method Not Allowed.
    /// </remarks>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="UpdateStatusResponse" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<UpdateStatusResponse?> GetUpdateStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Получает информацию о KSN.
    /// </summary>
    /// <param name="cancellationToken">Экземпляр токена отмены операции.</param>
    /// <returns>
    ///     Задача, представляющая асинхронную операцию и содержащая результат
    ///     этой операции <see cref="GetKsnInfoAsync" /> или <see langword="null" />, если HTTP Code = 404.
    /// </returns>
    public Task<KsnInfoResponse?> GetKsnInfoAsync(CancellationToken cancellationToken = default);
}