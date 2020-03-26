using System.Runtime.Serialization;

namespace ViennaNET.Word.Tests.Documents
{
  [DataContract]
  public class FormatTextTestDocument : Document
  {
    public FormatTextTestDocument(string name, string value)
    {
      FormatTextState = (name, value);
    }

    public (string name, string value) FormatTextState { get; }

    public override string FileName => nameof(FormatTextTestDocument);

    protected override void Fill()
    {
      FillSdtElement(FormatTextState.name, FormatTextState.value);
    }
  }
}