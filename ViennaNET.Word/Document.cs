using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ViennaNET.Utils;
using ViennaNET.Word.Barcode;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using W14 = DocumentFormat.OpenXml.Office2010.Word;

[assembly: InternalsVisibleTo("ViennaNET.Word.Tests")]

namespace ViennaNET.Word
{
  /// <summary>
  /// Предоставляет базовую реализацию для расширяемой модели документов Open Xml.
  /// </summary>
  /// <remarks>
  /// <para>
  /// В данном классе, реализован паттер шаблонный метод.
  /// </para>
  /// <para>
  /// Реализуйте метод <see cref="Fill()"/> который будет вызван при обращении к фабрике документов.
  /// </para>
  /// </remarks>
  public abstract class Document : IDisposable
  {
    private readonly List<BookmarkStart> _bookmarks;
    private readonly List<SdtElement> _elemetns;

    private MainDocumentPart _mainDocumentPart;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Document"/>.
    /// </summary>
    protected Document()
    {
      _bookmarks = new List<BookmarkStart>();
      _elemetns = new List<SdtElement>();
    }

    /// <summary>
    /// Выполняет поиск стандартных элементов в документе по значению <paramref name="name"/>
    /// и заполняет их значением <c><paramref name="value"/>.ToString()</c>.
    /// Если элементы не найдены, действие не выполняется.
    /// </summary>
    /// <param name="name">Имя целевого(ых) стандартных элементов.</param>
    /// <param name="value">Ссылка на объект, строковое представление которого необходимо поместить в элемент.</param>
    /// <exception cref="InvalidSdtElementException">
    /// Возникает если дерево дочерних элементов <see cref="SdtElement"/> содержит недопустимые элементы
    /// или отсутствует обязательный дочерний элемент.
    /// </exception>
    protected void FillSdtElement(string name, object value)
    {
      name.ThrowIfNullOrWhiteSpace(nameof(name));
      value.ThrowIfNull(nameof(value));

      try
      {
        foreach (SdtElement element in _elemetns.Where(element => element.SdtProperties.GetFirstChild<SdtAlias>()?.Val == name))
        {
          var newContentBlock = new SdtContentRun();
          Run run = newContentBlock.AppendChild(new Run());

          if (element.SdtProperties.GetFirstChild<RunProperties>() is RunProperties properties)
          {
            run.AppendChild(properties.CloneNode(true));
          }

          element.SdtProperties.RemoveAllChildren<SdtPlaceholder>();
          run.AppendChild(new Text(value.ToString()));
          element.ReplaceChild(newContentBlock, element.LastChild);
        }
      }
      catch (InvalidOperationException)
      {
        throw new InvalidSdtElementException(name);
      }
    }

    /// <summary>
    /// Выполняет поиск стандартных элементов, представляющих флажки в документе,
    /// по значению параметра <paramref name="name"/> и заполняет их значением параметра <paramref name="value"/>.
    /// Если элементы не найдены, действие не выполняется.
    /// </summary>
    /// <param name="name">Имя целевого(ых) стандартных элементов.</param>
    /// <param name="value">Значение для вставки в стандартные элементы.</param>
    /// <exception cref="InvalidSdtElementException">
    /// Возникает если дерево дочерних элементов <see cref="SdtElement"/> содержит недопустимые элементы
    /// или отсутствует обязательный дочерний элемент.
    /// </exception>
    protected void FillSdtElement(string name, bool value)
    {
      name.ThrowIfNullOrWhiteSpace(nameof(name));

      try
      {
        foreach (SdtElement element in _elemetns.Where(element => element.SdtProperties.GetFirstChild<SdtAlias>()?.Val == name))
        {
          if(element.SdtProperties.GetFirstChild<W14.SdtContentCheckBox>() is W14.SdtContentCheckBox contentCheckBox)
          {
            contentCheckBox.Checked = new W14.Checked()
            {
              Val = value ? W14.OnOffValues.One : W14.OnOffValues.Zero
            };

            var sdtContentBlock = new SdtContentBlock();
            Run run = sdtContentBlock.AppendChild(new Run());

            if (element.SdtProperties.GetFirstChild<RunProperties>() is RunProperties properties)
            {
              run.AppendChild(properties.CloneNode(true));
            }

            element.SdtProperties.RemoveAllChildren<SdtPlaceholder>();
            run.AppendChild(new Text(contentCheckBox.Checked.Val == W14.OnOffValues.One ? "☒" : "☐"));
            element.ReplaceChild(sdtContentBlock, element.LastChild);
          }
        }
      }
      catch (InvalidOperationException)
      {
        throw new InvalidSdtElementException(name);
      }
    }

    /// <summary>
    /// Выполняет поиск стандартных элементов, представляющих элемент даты и времени в документе,
    /// по значению параметра <paramref name="name"/> и заполняет их значением параметра <paramref name="value"/>.
    /// Если элементы не найдены, действие не выполняется.
    /// </summary>
    /// <param name="name">Имя целевого(ых) стандартных элементов.</param>
    /// <param name="value">Значение для вставки в стандартные элементы.</param>
    /// <exception cref="InvalidSdtElementException">
    /// Возникает если дерево дочерних элементов <see cref="SdtElement"/> содержит недопустимые элементы
    /// или отсутствует обязательный дочерний элемент.
    /// </exception>
    protected void FillSdtElement(string name, DateTime value)
    {
      name.ThrowIfNullOrWhiteSpace(nameof(name));

      try
      {
        foreach (SdtElement element in _elemetns.Where(element => element.SdtProperties.GetFirstChild<SdtAlias>()?.Val == name))
        {
          if (element.SdtProperties.GetFirstChild<SdtContentDate>() is SdtContentDate contentDate)
          {
            contentDate.FullDate = new DateTimeValue(value);

            var sdtContentBlock = new SdtContentBlock();
            Run run = sdtContentBlock.AppendChild(new Run());

            if (element.SdtProperties.GetFirstChild<RunProperties>() is RunProperties properties)
            {
              run.AppendChild(properties.CloneNode(true));
            }

            element.SdtProperties.RemoveAllChildren<SdtPlaceholder>();
            run.AppendChild(new Text(contentDate.FullDate.Value.ToString(contentDate.DateFormat.Val)));
            element.ReplaceChild(sdtContentBlock, element.LastChild);
          }
        }
      }
      catch (InvalidOperationException)
      {
        throw new InvalidSdtElementException(name);
      }
    }

    /// <summary>
    /// Выполняет поиск стандартных элементов в документе по значению <paramref name="name"/>.
    /// Генерирует изображение в формате png, представляющее штрих код соответствующий страндарту Code128,
    /// на основе значения <c><paramref name="value"/>.ToString()</c>.
    /// Извлекает из наденых элементов дочерний элемент, представляющий контейнер изображения
    /// и заполняет сгенерированным изображением.
    /// </summary>
    /// <param name="name">Имя целевого(ых) стандартных элементов.</param>
    /// <param name="value">
    /// Ссылка на объект, строковое представление которого необходимо необходимо закодировать.
    /// </param>
    /// <exception cref="InvalidSdtElementException">
    /// Возникает если дерево дочерних элементов <see cref="SdtElement"/> содержит недопустимые элементы
    /// или отсутствует обязательный дочерний элемент.
    /// </exception>
    protected void FillBarcode(string name, object value)
    {
      name.ThrowIfNullOrWhiteSpace(nameof(name));
      value.ThrowIfNull(nameof(value));

      try
      {
        var imageStream = new MemoryStream();
        var relationshipId = new StringValue(string.Empty);

        using (System.Drawing.Image barcodeImage = new Code128().Barcode(value.ToString(), false, BarWeight.Double, 40, true))
        {
          barcodeImage.Save(imageStream, ImageFormat.Png);
          imageStream.Position = 0;
        }

        foreach (SdtElement element in _elemetns.Where(element => element.SdtProperties.GetFirstChild<SdtAlias>()?.Val == name))
        {
          relationshipId = element.Descendants<Blip>().Single().Embed;

          FeedDataIfExistImagePart(_mainDocumentPart, relationshipId, imageStream);

          foreach (FooterPart footerPart in _mainDocumentPart.FooterParts)
          {
            FeedDataIfExistImagePart(footerPart, relationshipId, imageStream);
          }

          foreach (HeaderPart headerPart in _mainDocumentPart.HeaderParts)
          {
            FeedDataIfExistImagePart(headerPart, relationshipId, imageStream);
          }
        }
      }
      catch (InvalidOperationException)
      {
        throw new InvalidSdtElementException(name);
      }
    }

    /// <summary>
    /// Выполняет поиск стандартных элементов в документе по значению <paramref name="name"/>.
    /// Извлекает из наденых элементов дочерний элемент, представляющий таблицу
    /// и заполняет её значением свойств объектов из коллекции <paramref name="value"/>.
    /// </summary>
    /// <param name="name">Имя целевого(ых) стандартных элементов.</param>
    /// <param name="value">Коллекция объектов, значениями свойств которых будут заполнены ячейки таблицы.</param>
    /// <param name="skipHeader">Значение, указывающее, что первая строка является заголовком.</param>
    /// <remarks>
    /// <para>Ячейки таблицы заполняются в порядке объявления свойств в типе <typeparamref name="T"/>.</para>
    /// <para>
    /// Если значение параметра <paramref name="skipHeader"/> = <see langword="true"/>,
    /// тогда первая страка считается заголовком таблицы и не заполняется
    /// (Предполагается, что заголовок заполнен на этапе верстки).
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidSdtElementException">
    /// Возникает если дерево дочерних элементов <see cref="SdtElement"/> содержит недопустимые элементы
    /// или отсутствует обязательный дочерний элемент.
    /// </exception>
    protected void FillSdtElement<T>(string name, IEnumerable<T> value, bool skipHeader) where T : class
    {
      name.ThrowIfNullOrWhiteSpace(nameof(name));
      value.ThrowIfNull(nameof(value));

      try
      {
        foreach (SdtElement element in _elemetns.Where(element => element.SdtProperties.GetFirstChild<SdtAlias>()?.Val == name))
        {
          Table table = element.Descendants<Table>().Single();
          WordTableWriter.Write(value, table, skipHeader);
        }
      }
      catch (InvalidOperationException)
      {
        throw new InvalidSdtElementException(name);
      }
    }

    /// <summary>
    /// Выполняет поиск закладки в документе по значению параметра <paramref name="name"/>,
    /// и заполняет её значением <c><paramref name="value"/>.ToString()</c>.
    /// Если закладка не найдена, действие не выполняется.
    /// </summary>
    /// <param name="name">Имя целевой закладки.</param>
    /// <param name="value">
    /// Ссылка на объект строковое представление которого необходимо поместить в закладку.
    /// </param>
    /// <exception cref="InvalidSdtElementException">
    /// Возникает если дерево дочерних элементов <see cref="SdtElement"/> содержит недопустимые элементы
    /// или отсутствует обязательный дочерний элемент.
    /// </exception>
    protected void FillBookmark(string name, object value)
    {
      name.ThrowIfNullOrWhiteSpace(nameof(name));
      value.ThrowIfNull(nameof(value));

      try
      {
        BookmarkStart targetBookmark = _bookmarks.SingleOrDefault(bookmark => bookmark.Name == name);
        targetBookmark.Do(container =>
        {
         container.Parent.InsertAfter(new Run(new Text(value.ToString())), container);
        });
      }
      catch (InvalidOperationException)
      {
        throw new InvalidSdtElementException(name);
      }
    }

    /// <summary>
    /// Выполняет поиск стандартных элементов в документе по значению <paramref name="name"/>
    /// и удаляет их из их родительских элементов.
    /// </summary>
    /// <param name="name">Имя целевого(ых) стандартных элементов.</param>
    /// <exception cref="InvalidSdtElementException">
    /// Возникает если дерево дочерних элементов <see cref="SdtElement"/> содержит недопустимые элементы
    /// или отсутствует обязательный дочерний элемент.
    /// </exception>
    protected void RemoveSdtElement(string name)
    {
      try
      {
        foreach (SdtElement element in _elemetns.Where(element => element.SdtProperties.GetFirstChild<SdtAlias>()?.Val == name))
        {
          if (element.Parent is object)
          {
            element.Remove();
          }
        }
      }
      catch (InvalidOperationException)
      {
        throw new InvalidSdtElementException(name);
      }
    }

    /// <summary>
    /// Метод осуществляющий заполнение коллекций стандартных элементов и закладок документа.
    /// </summary>
    /// <example>
    /// <code>
    /// protected override void Fill()
    /// {
    ///   var isNeedRemovingElements = true;
    ///   
    ///   FillSdtElement("TextElement", "Текст");
    ///   FillSdtElement("CheckBoxElement", true);
    ///   FillSdtElement("DateTimeElement", DateTime.Today);
    ///   FillSdtElement("TableElement", new List{object}() { "Елемент 1", "Елемент 2" }, skipHeader:true);
    ///   FillBarcode("BarCodeImageElement", "123ABC");
    ///   FillBookmark("BookmarkName", "BookmarkValue");
    ///   if(isNeedRemovingElements)
    ///   {
    ///     RemoveSdtElement("ItemToBeDeleted");
    ///   }
    /// }
    /// </code>
    /// </example>
    protected abstract void Fill();

    /// <summary>
    /// Получает значение представляющее полный путь для сохранения документа.
    /// </summary>
    public abstract string FileName { get; }
    
    internal void Fill(MainDocumentPart mainDocumentPart)
    {
      _mainDocumentPart = mainDocumentPart;
      _elemetns.AddRange(mainDocumentPart.Document.Descendants<SdtElement>());
      _bookmarks.AddRange(mainDocumentPart.Document.Descendants<BookmarkStart>());

      foreach (FooterPart footerPart in mainDocumentPart.FooterParts)
      {
        _elemetns.AddRange(footerPart.Footer.Descendants<SdtElement>());
        _bookmarks.AddRange(footerPart.Footer.Descendants<BookmarkStart>());
      }
      foreach (HeaderPart headerPart in mainDocumentPart.HeaderParts)
      {
        _elemetns.AddRange(headerPart.Header.Descendants<SdtElement>());
        _bookmarks.AddRange(headerPart.Header.Descendants<BookmarkStart>());
      }

      Fill();
    }

    /// <summary>
    /// Освобождает и сбрасывает управляемые ресурсы потребляемые типом.
    /// </summary>
    public void Dispose()
    {
      _elemetns.Clear();
      _bookmarks.Clear();
      _mainDocumentPart = null;
    }

    private void FeedDataIfExistImagePart(OpenXmlPart part, string id, MemoryStream stream)
    {
      try
      {
        if (part.GetPartById(id) is ImagePart imagePart)
        {
          var destStream = new MemoryStream();
          stream.CopyTo(destStream);
          destStream.Position = 0;
          stream.Position = 0;
          imagePart.FeedData(destStream);
        }
      }
      catch (ArgumentOutOfRangeException)
      {
        return;
      }
    }
  }
}