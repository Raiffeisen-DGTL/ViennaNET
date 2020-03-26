using System;
using ViennaNET.Word.Tests.Documents;
using DocumentFormat.OpenXml.Packaging;
using NUnit.Framework;

namespace ViennaNET.Word.Tests
{
  internal class DebugDocument : Document
  {
    public override string FileName { get; }

    protected override void Fill()
    {
      //Алгоритм заполнения отладочного документа.
    }
  }

  [TestFixture(Category = "Debug", TestOf = typeof(WordprocessingDocumentFactory))]
  internal class WordDocumentFactoryTests
  {
    [Test]
    [Explicit]
    public void CreateTest()
    {
      var factory = new WordprocessingDocumentFactory();
      var document = new DebugDocument();

      using (var package = factory.Create(document, @"Путь до файла шаблона .dotx или документа .docx"))
      {
      }
    }
  }
}