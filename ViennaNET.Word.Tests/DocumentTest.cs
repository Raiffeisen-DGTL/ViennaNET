using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ViennaNET.Word;
using ViennaNET.Word.Tests.Documents;
using ViennaNET.Word.Tests.Properties;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using NUnit.Framework;
using static ViennaNET.Word.Tests.Documents.TableTestDocument;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(ViennaNET.Word.Document))]
  public partial class DocumentTests
  {
    [TestCase("Checkbox", false, ExpectedResult = "☐",
      Description = "Изменение знаячения стандартного элемента 'Флаг' на 'Не выбран'.")]
    [TestCase("Checkbox", true, ExpectedResult = "☒",
      Description = "Изменение знаячения стандартного элемента 'Флаг' на 'Выбран'.")]
    [TestCase("", true, ExpectedResult = null,
      Description = "Проверяет что не возможно изменить значение, если имя стандартного элемента пустая строка.")]
    [TestCase(" ", false, ExpectedResult = null,
      Description = "Проверяет что не возможно изменить значение, если имя стандартного элемента состоит из пробела.")]
    [TestCase("     ", true, ExpectedResult = null,
      Description = "Проверяет что не возможно изменить значение, если имя стандартного элемента состоит из пробелов.")]
    [TestCase(null, true, ExpectedResult = null,
      Description = "Проверяет что не возможно изменить значение, если имя стандартного элемента не определено.")]
    [Test]
    public string Fill(string name, bool value)
    {
      var document = new CheckboxTestDocument(name, value);
      var xDocument = XDocument.Parse(Resources.CheckboxTestDocument);
      using (var wordPackage = WordprocessingDocument.FromFlatOpcDocument(xDocument))
      {
        var body = wordPackage.MainDocumentPart.Document.Body;
        string actual = null;

        Assert.That(() =>
        {
          document.Fill(wordPackage.MainDocumentPart);
          actual = body.Descendants<SdtElement>()
            .Where(element => element.SdtProperties.GetFirstChild<SdtAlias>().Val == name)
            .Single()
            .Descendants<Text>()
            .First()
            .Text;

        }, Throws.Nothing | Throws.InstanceOf<ArgumentNullException>() | Throws.InstanceOf<InvalidSdtElementException>());

        return actual;
      }
    }

    [TestCase("RichText", "test", ExpectedResult = "test",
      Description = "Вставка заданного текста в документ")]
    [TestCase("RichText", "", ExpectedResult = "",
      Description = "Вставка пустой строки в документ")]
    [TestCase("RichText", null, ExpectedResult = null,
      Description = "Проверяет что невозможно вставить неопределённое значение в документ.")]
    [TestCase("", " ", ExpectedResult = null,
      Description = "Проверяет что не возможно вставить текст в документ, если имя стандартного элемента пустая строка.")]
    [TestCase(" ", "test", ExpectedResult = null,
      Description = "Проверяет что не возможно вставить текст в документ, если имя стандартного элемента состоит из пробелов.")]
    [TestCase(null, "    ", ExpectedResult = null,
      Description = "Проверяет что не возможно вставить текст в документ, если имя стандартного элемента не определено.")]
    [Test]
    public string Fill(string name, string value)
    {
      var document = new FormatTextTestDocument(name, value);
      var xDocument = XDocument.Parse(Resources.RichTextTestDocument);
      string actual = null;

      using (var wordPackage = WordprocessingDocument.FromFlatOpcDocument(xDocument))
      {
        var body = wordPackage.MainDocumentPart.Document.Body;
        Assert.That(() =>
        {
          document.Fill(wordPackage.MainDocumentPart);
          actual = body.Descendants<SdtElement>()
            .Where(element => element.SdtProperties.GetFirstChild<SdtAlias>()?.Val == name)
            .SingleOrDefault()
            ?.Descendants<Text>()
            ?.First()
            ?.Text;

        }, Throws.Nothing | Throws.InstanceOf<ArgumentNullException>() | Throws.InstanceOf<InvalidSdtElementException>());

        return actual;
      }
    }

    [TestCase("Date", "12.22.1993", ExpectedResult = "22.12.1993",
      Description = "Вставка даты в формате (mm.dd.yyyy) в документ, учитывая формат заданный на уровне разметки в документе.")]
    [TestCase("Date", "12/22/1993", ExpectedResult = "22.12.1993",
      Description = "Вставка даты в формате (mm/dd/yyyy) в документ, учитывая формат даты заданный на уровне разметки в документе.")]
    [TestCase("", "01.01.0001", ExpectedResult = null,
      Description = "Проверяет что не возможно вставить дату в документ, если имя стандартного элемента пустая строка.")]
    [TestCase(" ", "01.01.0001", ExpectedResult = null,
      Description = "Проверяет что не возможно вставить дату в документ, если имя стандартного элемента состоит из пробелов.")]
    [TestCase(null, "01.01.0001", ExpectedResult = null,
      Description = "Проверяет что не возможно вставить дату в документ, если имя стандартного элемента не определено.")]
    [Test]
    public string Fill(string name, DateTime value)
    {
      var document = new DateTestDocument(name, value);
      var xDocument = XDocument.Parse(Resources.DateTestDocument);
      string actual = null;

      using (var wordPackage = WordprocessingDocument.FromFlatOpcDocument(xDocument))
      {
        var body = wordPackage.MainDocumentPart.Document.Body;
        Assert.That(() =>
        {
          document.Fill(wordPackage.MainDocumentPart);
          actual = body.Descendants<SdtElement>()
            .Single(block => block.Descendants<SdtAlias>()
            .First().Val == name)
            .Descendants<Text>()
            .First()
            .Text;
        }, Throws.Nothing | Throws.InstanceOf<ArgumentNullException>() | Throws.InstanceOf<InvalidSdtElementException>());

        return actual;
      }
    }

    [TestCase("Image", 
      Description = "Вставка изображения в документ.",
      IgnoreReason = "В нашем docker образе для сборок нет библиотеки libdl " +
      "https://github.com/dotnet/dotnet-docker/issues/618. В остальном кейс рабочий. (НЕ УДАЛЯТЬ)")]
    [TestCase("",
      Description = "Проверяет что не возможно вставить изображение в документ, если имя стандартного элемента пустая строка.")]
    [TestCase(" ",
      Description = "Проверяет что не возможно вставить изображение в документ, если имя стандартного элемента состоит из пробелов.")]
    [TestCase(null,
      Description = "Проверяет что не возможно вставить изображение в документ, если имя стандартного элемента не определено.")]
    [Test]
    public void Fill(string name)
    {
      var value = name is null ? null : "TestBarCodeData012938";

      var document = new ImageTestDocument(name, value);
      var xDocument = XDocument.Parse(Resources.ImageTestDocument);

      using (var package = WordprocessingDocument.FromFlatOpcDocument(xDocument))
      {
        var body = package.MainDocumentPart.Document.Body;

        try
        {
          document.Fill(package.MainDocumentPart);

          if (name == "Image")
          {
            var blip = body.Descendants<SdtAlias>()
            .FirstOrDefault(aliase => aliase.Val == name)?.Parent?.Parent
            .Descendants<Blip>()
            .SingleOrDefault();

            Assert.That(blip, Is.Not.Null.And.InstanceOf<Blip>() | Is.Null,
              $"У каждого {nameof(SdtElement)} c {nameof(SdtAlias)}.{nameof(SdtAlias.Val)}={name} " +
              $"должен быть один вложенный элемент {nameof(Blip)}.");
            Assert.That(
              () => package.MainDocumentPart.GetPartById(blip.Embed),
              Is.Not.Null.And.InstanceOf<ImagePart>() | Is.Null,
              $"В тестовом документе должен быть объявлен элемент ImagePart с Relationship Id={blip.Embed}");

            var imagePart = package.MainDocumentPart.GetPartById(blip.Embed);
            Assert.NotNull(imagePart);
            Assert.AreEqual(
              "image/png", imagePart.ContentType, $"В тестовом документе должен быть объявлен элемент ImagePart с ContentType=image/png");

            using (var sourceStreamReader = new StreamReader(new MemoryStream(Convert.FromBase64String(Resources.TestImageData))))
            {
              using (var destinationStreamReader = new StreamReader(imagePart.GetStream()))
              {
                Assert.That(
                  () => destinationStreamReader.ReadToEnd(),
                  Is.EqualTo(sourceStreamReader.ReadToEnd()),
                  "Исходное изображение не соответствует полученному из документа.");
              }
            }
          }
        }
        catch(Exception e) when (e is ArgumentNullException || e is InvalidSdtElementException)
        {
          Assert.Pass();
        }
      }
    }

    [TestCase("Table", 1, "test", true)]
    [TestCase("Table", 0, default(string), default(bool))]
    [TestCase("", default(int), default(string), default(bool))]
    [TestCase(" ", default(int), default(string), default(bool))]
    [TestCase(null, default(int), default(string), default(bool))]
    [Test]
    public void Fill(string name, int id, string firstName, bool gender)
    {
      var row = new TestTableRow(name, id, firstName, gender);

      var document = new TableTestDocument(name, new List<object>()
      {
        row
      });

      var xDocument = XDocument.Parse(Resources.TableTestDocument);

      using (var wordPackage = WordprocessingDocument.FromFlatOpcDocument(xDocument))
      {
        var body = wordPackage.MainDocumentPart.Document.Body;

        Assert.That(() =>
        {
          document.Fill(wordPackage.MainDocumentPart);
        }, Throws.Nothing | Throws.InstanceOf<ArgumentNullException>());
      }
    }

    [TestCase("TestBookmark", "test", ExpectedResult = "test",
      Description = "Вставка заданного текста в закладку в документе")]
    [TestCase("TestBookmark", "", ExpectedResult = "",
      Description = "Вставка пустой строки в закладку в документе")]
    [TestCase("TestBookmark", null, ExpectedResult = null,
      Description = "Проверяет что невозможно вставить неопределённое значение в закладку в документе.")]
    [TestCase("", " ", ExpectedResult = null,
      Description = "Проверяет что не возможно вставить текст в закладку в документе, если имя стандартного элемента пустая строка.")]
    [TestCase(" ", "test", ExpectedResult = null,
      Description = "Проверяет что не возможно вставить текст в закладку в документе, если имя стандартного элемента состоит из пробелов.")]
    [TestCase(null, "    ", ExpectedResult = null,
      Description = "Проверяет что не возможно вставить текст в закладку в документе, если имя стандартного элемента не определено.")]
    [Test]
    public string FillBookmark(string name, string value)
    {
      var document = new BookmarkTestDocument(name, value);
      var xDocument = XDocument.Parse(Resources.BookmarkTestDocument);
      string actual = null;

      using (var wordPackage = WordprocessingDocument.FromFlatOpcDocument(xDocument))
      {
        var body = wordPackage.MainDocumentPart.Document.Body;
        Assert.That(() =>
        {
          document.Fill(wordPackage.MainDocumentPart);
          actual = body.Descendants<BookmarkStart>().SingleOrDefault(bookmark => bookmark.Name == name)
            ?.NextSibling()
            ?.Descendants<Text>()
            ?.SingleOrDefault()
            ?.Text;

        }, Throws.Nothing | Throws.InstanceOf<ArgumentNullException>() | Throws.InstanceOf<InvalidSdtElementException>());

        return actual;
      }
    }
  }
}