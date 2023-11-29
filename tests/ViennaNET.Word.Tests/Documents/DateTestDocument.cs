using System;

namespace ViennaNET.Word.Tests.Documents
{
  internal class DateTestDocument : Document
  {
    public DateTestDocument(string name, DateTime value)
    {
      DateState = (name, value);
    }

    public (string name, DateTime value) DateState { get; }
    public override string FileName => nameof(DateTestDocument);

    protected override void Fill()
    {
      FillSdtElement(DateState.name, DateState.value);
    }
  }
}