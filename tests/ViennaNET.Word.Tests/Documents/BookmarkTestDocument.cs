namespace ViennaNET.Word.Tests.Documents
{
  internal class BookmarkTestDocument : Document
  {
    public BookmarkTestDocument(string name, string value)
    {
      FormatTextState = (name, value);
    }

    public (string name, string value) FormatTextState { get; }
    public override string FileName => nameof(CheckboxTestDocument);

    protected override void Fill()
    {
      FillBookmark(FormatTextState.name, FormatTextState.value);
    }
  }
}