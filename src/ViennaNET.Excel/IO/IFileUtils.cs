namespace ViennaNET.Excel.IO;

/// <summary>
///     Бесполезный интерфейс, перенесённый из ViennaNET.FileUtils.Interfaces.
///     Следует удалить в пользу нативного System.IO;
/// </summary>
[Obsolete("Тип будет удалён в пользу нативного System.IO API, воздержитесь от использования.")]
public interface IFileUtils
{
    /// <summary>
    ///     Возвращает <see cref="Stream" />, содержащий данные из файла <paramref name="fileName" />.
    /// </summary>
    /// <param name="fileName">Путь к пакету таблиц.</param>
    /// <returns>Экземпляр <see cref="Stream" /> содержащий данные.</returns>
    Stream GetStreamFromFile(string fileName);
}