using System.Diagnostics.CodeAnalysis;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace ViennaNET.Word
{
  /// <summary>
  ///   Реализует логику создания пакетов <see cref="WordprocessingDocument" /> на основе шаблонов печатных форм.
  /// </summary>
  [ExcludeFromCodeCoverage(Justification = "Требуется создание интеграционных тестов.")]
  public sealed class WordprocessingDocumentFactory : IOpenXmlPackageFactory
  {
    /// <inheritdoc />
    public IDisposable Create<T>(T instance, MemoryStream templateContent) where T : Document
    {
      var package = WordprocessingDocument.Create(templateContent, WordprocessingDocumentType.Document, true);
      var fileName = string.IsNullOrEmpty(instance.FileName)
        ? Path.Combine(Path.GetTempPath(), $"{instance.GetType().Name} {DateTime.Now:dd.MM.yyyy hh.mm.ss}.docx")
        : instance.FileName;

      instance.Fill(package.MainDocumentPart!);
      return new CreateOpenXmlPackageContext(package.Clone(fileName), instance);
    }

    /// <inheritdoc />
    public IDisposable Create<T>(T instance, string templateFileName) where T : Document
    {
      var package = WordprocessingDocument.CreateFromTemplate(templateFileName, false);
      var fileName = string.IsNullOrEmpty(instance.FileName)
        ? Path.Combine(Path.GetTempPath(), $"{instance.GetType().Name} {DateTime.Now:dd.MM.yyyy hh.mm.ss}.docx")
        : instance.FileName;

      instance.Fill(package.MainDocumentPart!);
      return new CreateOpenXmlPackageContext(package.Clone(fileName), instance);
    }
  }
}