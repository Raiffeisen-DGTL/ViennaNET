using System.Diagnostics.CodeAnalysis;

namespace ViennaNET.Excel.IO;

/// <summary>
///     Реализация бесполезного <see cref="IFileUtils" />, перенесена из ViennaNET.FileUtils.Interfaces.
///     Следует удалить в пользу нативного System.IO;
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Не содержит логики, пригодной для модульного тестирования.")]
[Obsolete("Тип будет удалён в пользу нативного System.IO API, воздержитесь от использования.")]
public sealed class FileUtils : IFileUtils
{
    /// <summary>
    ///     Возвращает <see cref="MemoryStream" />, содержащий данные из файла <paramref name="fileName" />.
    /// </summary>
    /// <param name="fileName">Путь к пакету таблиц.</param>
    /// <returns>Экземпляр <see cref="MemoryStream" /> содержащий данные.</returns>
    public Stream GetStreamFromFile(string fileName)
    {
        return new MemoryStream(File.ReadAllBytes(fileName));
    }
}