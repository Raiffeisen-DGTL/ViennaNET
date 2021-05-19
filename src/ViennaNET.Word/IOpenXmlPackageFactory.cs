using System;
using System.IO;

namespace ViennaNET.Word
{
  /// <summary>
  /// Представляет фабрику по созданию пакета open xml.
  /// </summary>
  public interface IOpenXmlPackageFactory
  {
    /// <summary>
    /// Создаёт на основании шаблона пакет open xml.
    /// </summary>
    /// <typeparam name="T">Тип реализующий <see cref="Document"/>.</typeparam>
    /// <param name="instance">
    /// Ссылка на объект класса представляющего описание ПФ и реализующего <see cref="Document"/>.
    /// </param>
    /// <param name="templateContent">Содержимое файла шаблона.</param>
    /// <returns>Ссылка на объект типа <see cref="CreateOpenXmlPackageContext"/>.</returns>
    IDisposable Create<T>(T instance, MemoryStream templateContent) where T : Document;

    /// <summary>
    /// Создаёт на основании шаблона пакет open xml.
    /// </summary>
    /// <typeparam name="T">Тип реализующий <see cref="Document"/>.</typeparam>
    /// <param name="instance">
    /// Ссылка на объект класса представляющего описание ПФ и реализующего <see cref="Document"/>.
    /// </param>
    /// <param name="templateFileName">Путь до файла шаблона.</param>
    /// <returns>Новый экземпляр типа реализующего <see cref="CreateOpenXmlPackageContext"/>.</returns>
    IDisposable Create<T>(T instance, string templateFileName) where T : Document;
  }
}