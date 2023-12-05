using System;
using DocumentFormat.OpenXml.Packaging;
using ViennaNET.Utils;

namespace ViennaNET.Word
{
  /// <summary>
  ///   Представляет контекст создания пакета OpenXml, инкапсулирующий логику освобождения ресурсов.
  /// </summary>
  internal class CreateOpenXmlPackageContext : IDisposable
  {
    private readonly Document _document;
    private readonly OpenXmlPackage _package;

    internal CreateOpenXmlPackageContext(OpenXmlPackage package, Document document)
    {
      _package = package.ThrowIfNull(nameof(package));
      _document = document.ThrowIfNull(nameof(document));
    }

    /// <inheritdoc />
    public void Dispose()
    {
      _document.Dispose();
      _package.Dispose();
    }
  }
}